using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation
{
    public const int V = 0;
    public const int X = 1;
    public const int C = 2;
    public const int Count = 3;

    // Describes one ship's position in a formation
    public struct Slot
    {
        public int forward;         // Index of the slot in front of this one, if any, in the owning Formation
        public int backward;        // Index of the slot behind this one, if any, in the owning Formation
        public int newIndex;        // 
        public int sourceIndex;
        public List<int> neighbors; // Index of all neighboring slots in the owning formation
        public float x, z;          // Position
    }

    // Precalculated formations, indexed by FormationType, then by number of ships
    static Formation[,] formations;

    public int type;
    public Slot[] slots;
    public int newSlot; // Slot that has no match in previous formation, added ship goes here
    public float shift; // For position matching, so if the whole formation shifts forwards the match still works
    public Vector3 center;

    public static Formation Get(int type, int numShips)
    {
        return formations[(int)type, numShips];
    }

    static Formation()
    {
        // Build all formations
        formations = new Formation[Count, Fleet.MaxShips + 1];

        // Build base information, just an ordered list of positions
        for (int numShips = 0; numShips <= Fleet.MaxShips; numShips++)
        {
            //
            // Type V
            //

            {
                Formation formation = new Formation(V, numShips);
                formations[V, numShips] = formation;

                const float space = 5.0f;

                int rows = Mathf.CeilToInt(Mathf.Sqrt(0.25f + 2.0f * numShips) - 0.5f);

                int size = rows;    // Number of ships per side of the current triangle
                int slot = 0;       // Index on the current triangle side
                int dir = 0;        // Direction of movement: 0=upleft, 1=right, 2=downleft
                int last = numShips - rows * (rows - 1) / 2; // Number of ships in the last row
                float z = (rows - 1) * space;
                formation.shift = z;
                // Need to add a z offset to the formation for point matching.
                float x = 0.0f;     // Current position
                for (int i = 0; i < numShips; i++)
                {
                    // Place the ship
                    formation.slots[i].x = x;
                    formation.slots[i].z = z;

                    switch (dir)
                    {
                        case 0:
                        {
                            // Move diagonally
                            x -= space * 0.5f;
                            z -= space;

                            // dir = 1 handles the entire back row
                            if (slot == size - 2)
                            {
                                dir = 1;
                                slot = 0;

                                // Special set up for back row
                                if (size == rows)
                                {
                                    int shift;
                                    if (size % 2 == 0)
                                    {
                                        // Even size
                                        shift = (size - ((last + 1) & ~1)) / 2;
                                    }
                                    else
                                    {
                                        // Odd size
                                        shift = (size + (last % 2) - 1 - last) / 2;
                                    }
                                    x += shift * space;
                                }
                            }
                            else
                            {
                                slot++;
                            }
                        }
                        break;

                        case 1:
                        {
                            x += space;

                            if (slot == last - 1)
                            {
                                // Finished with the partial row, now do the last full row
                                x = (4 - rows) * space * 0.5f;
                                z = space;
                                slot = 1;
                                last = -1;
                                size--;
                            }
                            else if (slot == size - 2)
                            {
                                // Row is full
                                slot = 0;
                                dir = 2;
                                last = -1;
                            }
                            else
                            {
                                if (size % 2 == 1 && last % 2 == 0 && slot == last / 2 - 1)
                                {
                                    // Skip center slot if number of ships is even and number of slots is odd
                                    x += space;
                                }

                                slot++;
                            }
                        }
                        break;

                        case 2:
                        {
                            if (slot == size - 2)
                            {
                                // Move to next inner triangle
                                x -= space * 0.5f;
                                z -= space;
                                dir = 0;
                                slot = 0;
                                size -= 3;
                            }
                            else
                            {
                                // Move diagonally
                                x -= space * 0.5f;
                                z += space;
                                slot++;
                            }
                        }
                        break;
                    }
                }
            }

            //
            // Type X
            //

            {
                Formation formation = new Formation(X, numShips);
                formations[X, numShips] = formation;

                const float space = 5.0f;
                const float diagSpace = space / Util.Sqrt2;

                int rows = (numShips + 1) / 2;
                int even = 1 - (numShips % 2);
                float center = diagSpace * ((rows - 1) / 2);    // z-axis center
                float offset = even * space * 0.5f;             // handle even/odd

                for (int i = 0; i < numShips; i++)
                {
                    int ring = (i + 2) / 4;
                    int slot = (i + 1 + even) % 4;
                    formation.slots[i].x = (2 * (slot % 2) - 1) * (offset + ring * diagSpace);
                    formation.slots[i].z = center + ((slot & ~1) - 1) * ring * diagSpace;
                    // TODO ordering
                }
            }

            //
            // Type C TODO 
            //
            {
                Formation formation = new Formation(C, numShips);
                formations[C, numShips] = formation;
            }
        }

        //
        // Build extra data about the formations
        //

        // TODO skip type C for now, currently not filled out
        for (int iType = 0; iType < C; iType++)
        {
            for (int iShips = 1; iShips <= Fleet.MaxShips; iShips++)
            {
                Formation formation = formations[iType, iShips];
                Formation lastFormation = formations[iType, iShips - 1];
                List<int> newSlotsSameRow = new List<int>();

                //
                // First pass over all slots: find neighbors and slots that exactly match ones in the previous formation
                //

                const float range = 8.0f;
                for (int i = 0; i < formation.slots.Length; i++)
                {
                    //
                    // Average position
                    //

                    formation.center += new Vector3(formation.slots[i].x, 0.0f, formation.slots[i].z);

                    //
                    // Neighbor information
                    //

                    for (int j = i + 1; j < formation.slots.Length; j++)
                    {
                        float dx = formation.slots[j].x - formation.slots[i].x;
                        float dz = formation.slots[j].z - formation.slots[i].z;
                        float dd = dx * dx + dz * dz;

                        if (dd < range * range)
                        {
                            formation.slots[i].neighbors.Add(j);
                            formation.slots[j].neighbors.Add(i);
                        }

                        if (dx == 0)
                        {
                            if (dz > 0)
                            {
                                formation.slots[i].forward = j;
                                formation.slots[j].backward = i;
                            }
                            else
                            {
                                formation.slots[j].forward = i;
                                formation.slots[i].backward = j;
                            }
                        }
                    }

                    //
                    // Slot map part one: exact matches + find new slot
                    //
                    {
                        // Search for an exact match to the previous formation
                        bool sameRow = false;   // Are there existing slots in the same row
                        for (int j = 0; j < lastFormation.slots.Length; j++)
                        {
                            float dz = Mathf.Abs(formation.slots[i].z - formation.shift - (lastFormation.slots[j].z - lastFormation.shift));
                            if (dz < 0.1f)
                            {
                                if (formation.slots[i].x == lastFormation.slots[j].x)
                                {
                                    formation.slots[i].sourceIndex = j;
                                    formation.slots[j].newIndex = i;
                                    break;
                                }
                                sameRow = true;
                            }
                        }

                        // If no match is found, then this is a candidate for new slot
                        if (formation.slots[i].sourceIndex == -1)
                        {
                            if (sameRow)
                            {
                                newSlotsSameRow.Add(i);
                            }
                            else
                            {
                                formation.newSlot = i;
                            }
                        }
                    }
                }

                formation.center *= 1.0f / iShips;

                //
                // Choose the new slot for this formation
                //

                if (formation.newSlot == -1)
                {
                    // Middle slot is new
                    newSlotsSameRow.Sort((i1, i2) => (formation.slots[i1].x < formation.slots[i2].x ? -1 : (formation.slots[i1].x == formation.slots[i2].x ? 0 : 1)));
                    formation.newSlot = newSlotsSameRow[newSlotsSameRow.Count / 2];
                }

                //
                // Second pass over all slots: find the closest match for any unmatched slot
                //

                for (int i = 0; i < formation.slots.Length; i++)
                {
                    // Skip slots that are already matched
                    if (i == formation.newSlot || formation.slots[i].sourceIndex != -1)
                    {
                        continue;
                    }

                    // Find the closest unmatched slot in the old formation
                    int closestIndex = -1;
                    float closestDistance = float.MaxValue;
                    for (int j = 0; j < lastFormation.slots.Length; j++)
                    {
                        if (formation.slots[j].newIndex != -1)
                        {
                            continue; // old formation slot is already matched
                        }

                        float dx = formation.slots[i].x - lastFormation.slots[j].x;
                        float dz = formation.slots[i].z - formation.shift - (lastFormation.slots[j].z - lastFormation.shift);
                        dz *= 3.0f; // Prefer to stay in same row
                        dx = Mathf.Abs(dx) + (((dx < 0.0f) == (formation.slots[i].x < 0.0f)) ? 1.0f : 0.0f); // Prefer to expand outwards

                        // Choose closest
                        float distance = dx * dx + dz * dz;
                        if (distance < closestDistance)
                        {
                            closestIndex = j;
                            closestDistance = distance;
                        }
                    }

                    Debug.Assert(closestIndex != -1);
                    formation.slots[i].sourceIndex = closestIndex;
                    formation.slots[closestIndex].newIndex = i;
                }
            }
        }
    }

    public Formation(int type, int numShips)
    {
        this.type = type;
        newSlot = -1;
        shift = 0.0f;
        slots = new Slot[numShips];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].neighbors = new List<int>();
            slots[i].forward = -1;
            slots[i].backward = -1;
            slots[i].newIndex = -1;
            slots[i].sourceIndex = -1;
        }
    }
}
