# Code Review

## Pokemon-Likeness / IP Terminology Review

Scope: searched production code and tests for explicit Pokemon names, Pokemon terminology, Pokemon-like progression terms, and story/gameplay language that could read too close to Pokemon. This is an engineering/product-risk review, not legal advice.

External reference points used for context:

- Official Pokemon site for `Pokemon Legends: Arceus`, which identifies `Arceus` as a Pokemon: https://legends.arceus.pokemon.com/en-us/pokemon/arceus/
- Official Pokemon Sword/Shield site describing Gym Leaders, Gyms, Trainers, and the Champion challenge structure: https://swordshield.pokemon.com/en-us/people-galar-region/gym-leaders/
- Official Pokemon Legends: Arceus gameplay page describing wild Pokemon encounters, battles, catching, Pokeballs, and six-Pokemon party limits: https://legends.arceus.pokemon.com/en-us/gameplay/

### High: Explicit Pokemon character name remains in production story text

`GameLoop` still uses `Arceus`, which is an actual Pokemon name and part of an official Pokemon game title.

Relevant code:

- `Arcadia_v2/Gameplay/GameLoop.cs:133`
- `Arcadia_v2/Gameplay/GameLoop.cs:140`
- `Arcadia_v2/Gameplay/GameLoop.cs:141`
- `Arcadia_v2/Gameplay/GameLoop.cs:143`

Examples:

- `PrintArceusChallenge`
- `// Prints the final challenge text before the Arceus encounter.`
- `Arceus Voice: I knew you would eventually find your way here.`

Impact: this is the clearest remaining direct Pokemon IP issue because it uses a named Pokemon in user-facing production text.

Recommended fix: replace `Arceus` with an original Arcadia-specific final entity name, and rename the method/comment accordingly. For example, use `Arcadia Voice`, `The Origin`, `The First Guardian`, or tie it directly to the final creature with `NU_DRAGON`.
Solved

### Medium: Core progression still strongly resembles Pokemon's Gym / Badge / Champion loop

The game still uses a cluster of Pokemon-associated progression terms: `Gym`, `GymLeader`, `Badge`, `Champion`, `trainer`, `region champion`, and "defeat the 4 gyms ... challenge the region champion."

Relevant code:

- `Arcadia_v2/Gameplay/GameState.cs`
- `Arcadia_v2/Gameplay/GameSetup.cs`
- `Arcadia_v2/Gameplay/GymFlow.cs`
- `Arcadia_v2/Gameplay/MovementFlow.cs`
- `Arcadia_v2/Gameplay/MenuFlow.cs`
- `Arcadia_v2/Saves/GameSaveState.cs`
- `Arcadia_v2/Saves/GameStateMapper.cs`

Impact: none of these words alone is as direct as `Arceus`, but the combined structure is very close to Pokemon's well-known loop: trainers defeat Gym Leaders, earn badges, and challenge a Champion.

Recommended fix: rebrand the progression vocabulary as an Arcadia-original system. For example:

- `Gym` -> `Trial`, `Sanctuary`, `Arena`, or `Challenge Hall`
- `GymLeader` -> `Warden`, `Keeper`, `Master`, or `Guardian`
- `Badge` -> `Sigil`, `Seal`, `Mark`, or `Crest`
- `Champion` -> `High Guardian`, `Arcadia Regent`, or `Grandmaster`
- `trainer` -> `handler`, `ranger`, `traveler`, or `guardian`

### Medium: Wild battle / catch / release / six-creature party loop is Pokemon-like

The wild encounter flow still uses Pokemon-like gameplay language and structure: wild creature attacks, the player defeats it, then chooses whether to catch it; if the party is full, the player can release one creature; party size is capped at six.

Relevant code:

- `Arcadia_v2/Gameplay/WildBattleFlow.cs`
- `Arcadia_v2/Battles/BattleEngine.cs`

Impact: this does not contain direct Pokemon names, but the combination of wild encounters, battling, catching after defeat, releasing party creatures, and a six-creature limit is recognizably Pokemon-like.

Recommended fix: decide whether the mechanic should be rebranded or redesigned. A low-risk terminology pass would replace `catch` with `befriend`, `recruit`, `rescue`, or `bond`, and replace `release` with `send home`, `return to habitat`, or `dismiss`. A stronger gameplay distinction would avoid "defeat then catch" and instead use a trust/bonding choice, quest reward, or sanctuary rescue system.

### Low: `Nucleon` is not Pokemon-owned, but it reads risky in this context

`Nucleon` is a normal science term, but in this creature-battling/nuclear-element game it may be read as Pokemon-adjacent because `Nucleon` is also associated with monster-collector fan-game terminology.

Relevant code:

- `Arcadia_v2/Gameplay/GameSetup.cs:90`
- `Arcadia_v2/Map/Room.cs:14`
- `Arcadia_v2/Map/Room.cs:21`
- `Arcadia_v2/Map/Map.cs:50`
- `Arcadia_v2/Map/Map.cs:57`
- `Arcadia_v2/Map/Map.cs:153`

Impact: this is lower risk than `Arceus` because it is a generic word, but the surrounding genre context makes it worth reconsidering.

Recommended fix: rename the towns/region references to a more original Arcadia-specific nuclear name, such as `Radion`, `Isotope Crossing`, `Corefall`, `Ashen Core`, or `New Radia`.

### Low: Test-only fixtures still include Pokemon-adjacent names and exact Pokemon badge text

Most of these are not production-facing, but they still exist in the repository and could leak through screenshots, docs, or public source.

Relevant examples:

- `UnitTest/PlayerTest.cs:13` uses `Professor's Lab`
- `UnitTest/PlayerTest.cs:31` uses player name `Red`
- `UnitTest/PlayerTest.cs:41` uses player name `Red`
- `UnitTest/PlayerTest.cs:43` uses `Boulder Badge`
- `UnitTest/SaveSystemTests.cs:129` uses player name `Red`
- `UnitTest/SaveSystemTests.cs:318` uses player name `Blue`
- `UnitTest/SaveSystemTests.cs:145` uses move name `TACKLE`

Impact: `Red`, `Blue`, `Professor's Lab`, and `TACKLE` are individually generic, but in this repo context they read Pokemon-adjacent. `Boulder Badge` is especially worth replacing because it is an exact Pokemon badge name.

Recommended fix: rename test fixtures to Arcadia-neutral names and terms, such as `Riley`, `Morgan`, `Maia's Stable`, `Stone Sigil`, and `Pounce`.

### Low: Historical review text itself still contains Pokemon terms

`CodeReview.md` intentionally preserves earlier findings that mention Pokemon terminology and examples. That is useful internally, but if this repository is published or shown externally, the review file itself still contains the words being audited.

Relevant code:

- `Arcadia_v2/CodeReview.md`

Impact: low for runtime behavior, but nonzero for public repository hygiene.

Recommended fix: if this repository will be public, either remove historical Pokemon wording after the cleanup is complete or move the review history into a private/internal note.

### Clean Areas

No remaining production matches were found for these direct Pokemon terms outside of the findings above: `Pokemon`, `Poke`, `pokeball`, `Pikachu`, `Charmander`, `Bulbasaur`, `Squirtle`, `Charizard`, `Mewtwo`, `Mew`, `Eevee`, or `Team Rocket`.

## Test Gaps

The current test suite is passing, but it does not yet cover several real user flows:

- Broader gameplay coverage for roster changes beyond the factory and map invariant tests.
- Null or partially missing save JSON lists, which could still escape the current save-load failure handling.

## Overall Result

The codebase is in a working state and the current tests pass. The main remaining risk is malformed or partially missing save JSON, which has less coverage than the core battle, movement, roster, and command flows. From the IP terminology pass, the highest priority cleanup is removing the explicit `Arceus` reference, followed by rebranding the Gym/Badge/Champion progression loop and the catch/release wild-creature language.
