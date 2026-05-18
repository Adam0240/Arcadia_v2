## Overview

I have redesigned my game map. Update the existing map so routes are now called roads, add the extra towns and areas, and update each room's directional connections based on the table below.

Use the exact room names shown here.

## Map Directions

1. Professor's Lab
   - North: Ikena
   - East: N/A
   - South: N/A
   - West: N/A
   - Note: This is the starting room. Once the player leaves, they cannot return. Ignore that design issue for now.

2. Ikena
   - North: The End
   - East: Road 6
   - South: Road 5
   - West: Road 1
   - Lock notes:
     - North to The End is locked until the region's guardian/champion has been defeated.
     - East to Road 6 is locked until 3 badges are obtained.
     - South to Road 5 is locked until the player has a Water-type Pokemon on their team.

3. Road 1
   - North: Road 8
   - East: Ikena
   - South: Road 2
   - West: N/A

4. Road 2
   - North: Road 1
   - East: N/A
   - South: Oak Pass
   - West: N/A

5. Oak Pass
   - North: Road 2
   - East: N/A
   - South: Road 3
   - West: N/A

6. Road 3
   - North: Oak Pass
   - East: N/A
   - South: Road 4
   - West: N/A

7. Road 4
   - North: Road 3
   - East: N/A
   - South: New Nucleon
   - West: N/A

8. New Nucleon
   - North: Road 4
   - East: Road 5
   - South: N/A
   - West: N/A
   - Lock notes:
     - East to Road 5 is locked until the player has a Water-type Pokemon on their team.

9. Road 5
   - North: Ikena
   - East: Nucleon
   - South: N/A
   - West: New Nucleon
   - Lock notes:
     - East to Nucleon is locked until 4 badges are obtained.

10. Road 6
    - North: Final Trials
    - East: N/A
    - South: Road 7
    - West: Ikena

11. Road 7
    - North: Road 6
    - East: N/A
    - South: Wyrmrest
    - West: N/A

12. Wyrmrest
    - North: Road 7
    - East: N/A
    - South: Mountains
    - West: N/A
    - Note: Dracoton has been renamed to Wyrmrest. Update the fourth gym leader's location to this room.

13. Mountains
    - North: Wyrmrest
    - East: N/A
    - South: Radioactive Way
    - West: N/A

14. Radioactive Way
    - North: Mountains
    - East: N/A
    - South: Nucleon
    - West: N/A

15. Nucleon
    - North: Radioactive Way
    - East: N/A
    - South: N/A
    - West: Road 5

16. Final Trials
    - North: Guardian's Tower
    - East: N/A
    - South: Road 6
    - West: N/A

17. Guardian's Tower
    - North: N/A
    - East: Final Trials
    - South: Ikena
    - West: Road 8

18. Road 8
    - North: Guardian's Tower
    - East: N/A
    - South: Road 1
    - West: N/A

19. The End
    - North: N/A
    - East: N/A
    - South: Ikena
    - West: N/A

## Implementation Notes

- Rename all existing `Route X` rooms to `Road X`.
- Add the new rooms: `Mountains`, `Radioactive Way`, `Nucleon`, `Final Trials`, `Guardian's Tower`, and `Road 8`.
- Replace `Dracoton` with `Wyrmrest` everywhere, including the fourth gym leader location.
- Preserve existing game behavior unless it conflicts with the map and lock rules above.
- If the code does not already support Water-type movement locks, add that behavior where movement permissions are checked.
- If any unit test check directions ensure they are updated to reflect the updated map. 
