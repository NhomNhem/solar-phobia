# Art Bible — Solar Phobia: Nắng Gắt

> **Status**: Complete
> **Created**: 2026-05-07
> **Art Director Sign-Off**: APPROVED 2026-05-07
> **Style**: Hand-painted watercolour with fire damage

---

## Section 1: Visual Identity Statement

**One-line Rule**:  
Every frame looks like a watercolour illustration that survived a fire — warm, textured, and human when you think, then scorched and bare when you run.

**Supporting Principles**:

1. **Tactile Deliberation** (Day/Night emotional contrast)  
   Warm, visible brushstrokes and soft edges during day phases signal safety and humanity.  
   *Design test: if a day-phase element looks photorealistic or digitally clean, push it toward visible stroke textures or softened silhouettes until it feels hand-touched.*

2. **Witnessed Choices** (Meaningful choices with immediate consequences)  
   Every survivor or corpse exists as a painted mark — strokes that remain on screen, on the ground, on the world.  
   *Design test: if a consequence doesn't leave a visible painted residue in the environment, it doesn't land. Add a mark, a smear, a colour bleed, or it didn't happen.*

3. **Heat and Void** (Emotional weight of sacrifice)  
   Day palette: scorched ochres, sun-bleached textures, oppressive golds. Night palette: ink-wash indigos, moonlit voids, deep blacks. The palette IS the emotion — no middle ground.  
   *Design test: if you're unsure whether to warm or cool a scene, ask — is the player deciding or escaping? Warmth means they still have time. Cold means time has run out.*

---

## Section 2: Mood & Atmosphere

### Core Philosophy: Permeable Boundaries
Day shapes feel like liquid held briefly in form — rounded corners, soft intersections, shapes that seem to flow. Night shapes fracture into hard edges, exposed scaffolding, forms that can't hold warmth.

### Character Silhouettes
- **Day**: Soft indefinite forms, rounded blobs, draped cloth curves, unfocused edges that bleed. Readable at thumbnail but organic at scale.
- **Night**: Exposed skeleton, skeletal fingers, hard angular limbs. Visible *too much* — stripped to structural core.

**Archetypes**: Survivor (shoulder drape, low center), Guardian (vertical emphasis), Lost One (asymmetric/broken), Harbinger (sharp projections)

### Environment Geometry
- **Day**: Organic/curved dominance — curved hillforms, rounded architectural elements. Nature remembers curves.
- **Night**: Angular/geometric dominance — sharp fractured planes, hard-edged shadows. Environment becomes antagonist.

### UI Shape Grammar
- **Day**: Rounded containers, soft shadows that pool like wet paint, wavering drawn borders
- **Night**: Sharp cutouts, singed edges, cracked fragments, shimmering text like heat distortion

### Hero vs Supporting Shapes
- **Day**: Player has largest silhouette, warmest values, most complex edges
- **Night**: Player is what's *left whole* in a field of debris — isolation through intact geometry

---

## Section 2: Color System

### Philosophy: Heat and Void
The palette IS the emotional state — there is no comfortable middle ground. Every color must answer one question: **is the player still human, or are they being consumed?**

---

### Primary Palette

| Color | Role | Meaning | Usage |
|-------|------|---------|-------|
| **Paper White** | Blank canvas, lost potential | Unmarked default — the world before fire, the player before choice. Faint warmth undertone (ivory, NOT pure white) | UI backgrounds, unvisited areas, safe zones, resolve states |
| **Burnt Sienna** | Active memory, recent sacrifice | Warmth with weight — someone choosing to stay. Red-orange-brown with texture | Day-phase characters, alive NPCs, touched objects, recent survivor marks |
| **Ochre Gold** | The cost of time, delayed consequence | Heavy amber — warmth that costs something. Desaturated, oppressive (NOT celebratory gold) | Selection UI, choice timers, saved/sheltered zones, the "golden hour" itself |
| **Charcoal Black** | Consumed, post-decision void | Not pure black — warm black with ember memories. The fire took something. It left this. | Night-phase ground, burned areas, lost NPCs, failed choices |
| **Ember Orange** | Active danger, immediate threat | Orange-red, pulsating — fire at the screen's edge. High saturation, warm temperature. The only light in night. | Chase sequences, enemy indicators, fire-damage effects |
| **Bruised Indigo** | Tipping point, between states | Blue-violet-gray — the dusk that doesn't want to commit. The moment still holds. | Transition zones, ChoiceLock state, warning indicators |
| **Canvas Cream** | Absence, erasure, nothing left | Off-white with faint warm tint — bare paper showing through. No evidence. No memory. | Death states, erase effects, true void |

### Semantic Vocabulary

- **Red/Orange family (Ember, Sienna)**: **Danger — active threat**  
  — *Why? Fire is the real enemy. The color of fire means you're being chased right now.*  
  — Backup: sharp silhouette spikes, proximity rumble sound, screen-edge vignette

- **Gold/Ochre**: **Cost — paid consequence**  
  — *Why? Gold means warmth you earned or warmth someone paid for. It carries weight — not celebratory, but weighted.*  
  — Backup: hourglass icon, ticking sound, number count (time remaining)

- **White/Paper family**: **Safety — still human**  
  — *Why? The unmarked canvas is who you were before choice. It's tender, it's blank, it's you.*  
  — Backup: soft pulse, gentle tone, checkmark icon

- **Indigo/Bruised**: **Warning — tipping point**  
  — *Why? The color of not-yet-night, not-yet-day. The held breath.*  
  — Backup: pulsing triangle, tension note building, countdown display

- **Black/Charcoal**: **Consumed — consequence enacted**  
  — *Why? Not evil — just what fire left behind. The world showing its bones.*  
  — Backup: silhouette outline that stays, absence visual (negative space), low rumble

- **Blue (any hue)**: **Never used**  
  — *Why? There is no cold safety in this world. The cold is the void. Blue = you are already gone.*

- **Pure White**: **Never used**  
  — *Why? The canvas is bare paper, not bright white. Everything carries warm undertone. Even absence has memory.*

---

### Biome/Area Temperature Rules

| State | Dominant Temperature | Contrast Level | Palette Collapse |
|-------|-------------------|---------------|-----------------|
| **DayService** | Warm (Sienna → Ochre) | Low (soft, dreamy) | Full palette visible |
| **ChoiceLock** | Transitional (Ochre → Indigo) | Rising | Palette narrowing |
| **NightSurvival** | Hot → Cold (Ember → Charcoal) | High (harsh) | Collapsed to 3-4 colors max |
| **Resolve** | Cool (Canvas) | Low | Restored palette - muted |

---

### UI Palette (Divergence from World Palette)

The UI lives in a different relationship to fire than the world does.

| UI Element | Day State | Night State | Rationale |
|-----------|----------|------------|-----------|
| **Text** | Paper White on transluscent sienna wash | Charcoal with ember-edge glow | Day: reads like ink on watercolor. Night: reads like ember through ash. |
| **Containers** | Soft rounded, wet-edge blur | Sharp cracked edges, heat-shimmer | Day: water holds shape. Night: heat breaks shape. |
| **Icons** | Brushstroke filled | Line-only silhouette | Day: pigment. Night: skeleton. |
| **Health/Stamina bars** | Ochre fill (wounded gold) | Ember pulse (dying glow) | Same bar, different fire state |
| **Selection** | Ochre glow | Indigo pulse | Time remaining vs time running out |

**Critical divergence**: UI never uses Black in day, never uses White in night. The UI must remain *legible* as a human interface even when the world becomes inhuman.

---

### Colorblind Safety

| Semantic Color | Problem For | Primary Backup | Secondary Backup | Implementation Priority |
|--------------|-----------|--------------|---------------|---------------------|
| **Ember (danger)** | Protanopia, Deuteranopia | **Sharp triangular silhouette** (jagged/spiking edge) | Proximity rumble + screen-edge heat vignette | Must have — danger must be unambiguous |
| **Ochre Gold (cost)** | Deuteranopia | **Hourglass icon** + time number | Ticking sound + value number display | Must have — currency/payment ambiguity = broken economy |
| **Indigo (warning)** | Tritanopia | **Pulsing diamond** shape | Tension audio swell + countdown text | Recommended — state transition critical |
| **White/Paper (safety)** | Protanopia | **Soft circular pulse** (breathing) | Checkmark icon + gentle chime | Should have — safety = player alive |
| **Charcoal (consumed)** | None — high contrast black reads | Silhouette outline that remains | Low priority — already reads as void |

**Total prohibition**: Never use color alone for any semantic meaning. Every color must have a shape or edge-quality partner. The watercolor texture itself counts as a secondary signifier — wet-bleed edges = warm/safe, cracked edges = consumed.

---

*Design test: If a player who sees in monochrome can still understand the game state through silhouette, edge-quality, and texture — the color system is working. If they need color to know "danger" from "cost," the system has failed.*

---

### DayService
- **Emotion**: Tender melancholy — a quiet grief for people you've barely met
- **Lighting**: Late afternoon, golden-hour warmth. Low contrast, soft shadows. Sun hangs heavy and amber, casting everything in honeyed light.
- **Atmosphere**: Placid, contemplative, human-scale, intimate, aching
- **Energy**: Measured. Time moves slowly. You can almost hear the clock ticking.
- **Visual Element**: Long shadows from village structures stretch across cobblestones. The watercolour texture is at its most lush here — visible brushstrokes, pigment bleed, paper grain.

### ChoiceLock
- **Emotion**: Held breath — the moment between exhale and action
- **Lighting**: Dusk tipping into blue hour. Colour temperature drops from amber to violet-indigo. Contrast rises as the sun dies.
- **Atmosphere**: Suspended, tightening, electric, watchful, grave
- **Energy**: Knotted. Stillness with weight.
- **Visual Element**: Character's shadow elongates dramatically. Watercolour begins to crack at edges — fine dark lines where heat would soon emerge. Sky bleeds from warm coral into bruised purple.

### NightSurvival
- **Emotion**: Primal terror — not fear of death, but of being consumed
- **Lighting**: Near-darkness. The only light comes from the burning horizon (orange-red, high contrast, harsh). Player is a silhouette against fire.
- **Atmosphere**: Scorched, skeletal, frantic, fractured, stripped
- **Energy**: Frenetic. Every frame is a sprint.
- **Visual Element**: World is partially consumed by fire-damage. Holes reveal bare paper. Movement trails blur like wet paint smearing. Palette collapses to black, ember-orange, white of exposed canvas.

### Resolve (Win)
- **Emotion**: Exhausted salvation — the fragile relief of being allowed to live
- **Lighting**: First grey light of dawn. Cold, desaturated, but present. Low contrast, diffuse.
- **Atmosphere**: Frail, raw, fragile, hallowed, quiet
- **Energy**: Coming down. Breath returning to normal.
- **Visual Element**: Village is half-burned, half-standing. Both intact watercolour and fire aftermath visible.

### Resolve (Lose)
- **Emotion**: Complete erasure — not pain, but absence
- **Lighting**: None. Total black. No light source remains.
- **Atmosphere**: Null, void, finished, still, gone
- **Energy**: None. Flatline.
- **Visual Element**: Watercolour entirely consumed. Bare paper remains — off-white with faint ember-glow at edges. No evidence you were ever there.

---

## Section 5: Character Design Direction

### Core Philosophy: Painted Marks That Remain or Burn

Characters exist as painted marks on the paper-scrying surface. In daylight they are controlled brushstrokes with visible texture; at night they burn at edges, losing definition.

### Player Character — The Surviving Mark
- **Silhouette**: Compact, legible, slightly abstracted humanoid. Readable against both bright day and dark night backgrounds.
- **Proportions**: Slightly elongated (longer torso, shorter limbs) — fragile, not powerful
- **Distinguishing traits**: 
  - No facial features — just darker brushstroke "mask"
  - Visible brushstroke texture on limbs
  - Leaves faint watercolor smear where walking (prominent in day, barely in night)
- **Color identity**: Ochre Gold (day) → Charcoal Black with Ember Orange edges (night)

### NPC/Soul Archetypes — Linh, Van, Minh

**Linh — The Watercolor Wash**
- Shape: Soft, flowing, permeable edges — wash that spread beyond boundary
- Silhouette: Rounded, indistinct, almost blob-like
- Day: Pale, translucent (Bruised Indigo wash)
- Night: Darkens to Charcoal Black but remains soft-edged
- At a glance: Most vaporous/ethereal — mistakable for shadow

**Van — The Ink Stroke**
- Shape: Sharp, angular, confident — single confident pen stroke
- Silhouette: Geometric, elongated, upright with squared shoulders
- Day: Bold Burnt Sienna with visible brush direction
- Night: Stroke splits into branching lines like struck match
- At a glance: Most solid/defined — person first, watercolor second

**Minh — The Stippled Impasto**
- Shape: Dense, textured, built-up — heavy pigment stacked
- Silhouette: Squat, compact, always slightly hunched
- Day: Heavy Ochre Gold with visible granulation
- Night: Impasto crusts and cracks, leaving gaps of bare paper
- At a glance: Roughest/heaviest texturally — physically present

### Enemy/Boss — Cá Ông Whale
- **Philosophy**: Not a character, a stain that keeps growing
- **Day**: Clearly readable whale silhouette in Bruised Indigo with soft feathered edges
- **Night**: Silhouette evaporates — edges lift off, leaving gaps and transparency
- Scale: Never fully visible — glimpses through water like wet-on-wet wash
- Eyes: Two simple dark spots (Charcoal Black) — no whites, no pupils

### Expression/Pose Style
- **No faces**: Expression through body tilt, limb position, edge behavior, color intensity
- **Day**: Subtle, restrained, slightly stiff like paint not fully dried
- **Night**: Exaggerated, urgent, limbs extend further, silhouettes more extreme
- **Burn communication**: Burning edges trail smoke-like pigment — how emotional state reads

### LOD Philosophy

| Tier | Distance | Treatment |
|------|----------|-----------|
| **Far** | 1-2cm screen height | Silhouette only — solid color blob |
| **Mid** | Gameplay distance | Shape + edge — brushstroke texture visible, no facial detail |
| **Close** | Cutscene | Full watercolor — paper texture, pigment granulation visible |

No popping — detail fades in/out across distance. At max distance, characters feel like color notes on the page.

---

## Section 6: Environment Design Language

### Core Philosophy: The Village as Witness
The environment is not scenery — it is the archaeological layer of a choice. Every structure, every surface, every absence tells the story of what was saved and what was burned. The Vietnamese fishing village aesthetic grounds the cosmic horror in human hands: these are not abstract horrors, but the remnants of someone's home, someone's livelihood, someone's prayers.

---

### 1. Architectural Style

#### Cultural Anchoring: Lý Sơn Fishing Village Morphology

The world recreates a stylized version of the coastal village where Tú failed his oath. Reference authenticity: **do not replicate, interpret.** Outsourced teams receive mood boards, not blueprints.

| Element | What It Is | Why It Works | Execution Guideline |
|---------|-----------|-------------|-------------------|
| **Rafter Frames** | Exposed wooden beam structures, triangular roof silhouettes | The skeleton of fishing village communal houses — familiar shape, now denuded | Day: warm wood grain texture. Night: angular black silhouettes against ember. Shape never varies between phases. |
| **Lantern Posts** | Vertical bamboo poles with paper lanterns suspended at intersections | Wayfinding in fishing villages; now the only warmth in night | Paper-textured mesh, warm sienna glow (Day). Night: lantern becomes ember source, single point of light in void. |
| **Nón Lá Canopy** | Conical leaves woven over makeshift stalls | Iconic Vietnamese form — organic dome against angular architecture | Curved silhouette in Day, sharp geometric shadow in Night. Texture: visible woven strands. |
| **Boat Hulls** | Weathered fishing boat hulls beached on land | The livelihood that was abandoned — symbol of Tú's guilt | Partial hulls only (1/3 visible). Day: warm ochre wood. Night: charred black remains showing internal ribs. |
| **Cổng Đá (Stone Gate)** | Low stone archways marking village thresholds | Transition between safe and dangerous | Always slightly too short — forces crouch. Day: moss-covered warm grey. Night: ink-black void passage. |
| **Altar Niches** | Small recessed wall cavities with incense holders | Ancestor worship — the spiritual weight Tú fled | Always at eye-level. Day: embers smoldering. Night: cold, empty, dark. |
| **Net Drying Racks** | A-frame bamboo frames with ghost nets hanging | Fishing village texture — vertical line chaos | Racks at 3 densities: sparse (safe zone), medium (transitional), dense (danger zone). Net = vertical edge grammar. |

#### World Culture Connection: Universal Fishing Village Typology
This aesthetic draws from a typology shared across coastal Asia and Mediterranean fishing communities:

- **Vietnam**: Nón lá conical roofs, cổng đá stone gates, boat hulls on beaches
- **Japan/Amami Islands**: Paper lantern waypoints, exposed beam frames
- **Greek Islands**: White-washed walls (absent — this world is post-burn, not sunny)  
- **Portuguese/Filipino fishing villages**: Net drying racks, exposed skeletons

**The shared language**: Vertical posts + horizontal surfaces + fabric/nets as weatherproofing = the fishing village vocabulary. Every Outsourced team receives 5 reference images, not architectural textbooks.

#### Don't Do: Replication
> **Rule**: No structure in this world was built to be beautiful. It was built to survive the sea.
> 
> *Design test: If an environment element looks like a tourist attraction, it doesn't belong. The village is functional, weathered, and broken.*

---

### 2. Texture Philosophy: Painted vs. PBR vs. Stylized

#### The Core Decision: Watercolour is Not a Shader

| Approach | Definition | Why It's Wrong for This Game | Why Watercolour Wins |
|----------|------------|---------------------------|-------------------|
| **PBR (Physically Based Rendering)** | Realistic material response — metal reflects, wood absorbs, stone is matte | Creates *distance*. The player observes the realism, not the emotion. PBR says "this is how the world looks." We need "this is what the world felt." | N/A — we reject PBR entirely. |
| **Stylized (Cel-shaded, flat)** | Hand-drawn outlines + limited color steps | Creates *safety*. Cel-shading reads as "intentional game." This world was not intentional — it was burned. Too clean. | N/A — cel-shading is too controlled. |
| **Painted** | Visible brushstroke, pigment bleed, paper grain, wet-edge chaos | ***This is the way***. The painting shows the hand that made it. Fire shows the hand that unmade it. | Every surface carries warmth or its absence. The texture IS the emotion. |

#### Technical Execution: "Painted" in Unity URP 2D

**Day Phase — Luxuriant Brushstroke**:
- Custom shader: Paper grain overlay (subtle, 5% opacity)
- Brushstroke normal map: Soft edges that catch light at grazing angles
- Pigment bleed: Color bleeding at geometry edges (inner glow, <3px)
- No hard outlines: Soft black wash at occlusion points only

**Night Phase — Charred Canvas**:
- Paper grain becomes exposed: Grain intensifies as surface disappears
- Brushstroke hardens: Wet paint dried into cracked glaze
- Burn edges: Black char creeping from corners, following stroke direction
- Bare paper: Holes reveal canvas texture underneath (not black — cream/off-white)

**Transition (ChoiceLock)**:
- Paint cracks: Fine dark lines appearing where wet stroke dried too fast
- Heat warp: Subtle vertex displacement along warm edges
- Ember bleed: Orange saturating into adjacent cool colors

#### Shader Budget: What Outsourcing Needs to Deliver

Every environment asset includes:

| Layer | Day State | Night State | Technical Method |
|-------|----------|------------|-----------------|
| **Base Color** | Full pigment, warm undertone | Desaturated, warm black | 2-frame sprite swap or shader property |
| **Normal** | Soft grain brushstroke | Cracked/hard grain | Normal map intensity boost |
| **Emission** | None (lit by environment) | Ember edges only (1-2px) | Emission channel on burn edges |
| **Mask** | Full | Partial (holes where burned) | Alpha cutout for bare paper |

> **Outsourcing Contract**: "Deliver Day-state sprites. Write clean shader code that transitions to Night state. Do not deliver two separate Day/Night art packages — deliver one system."

---

### 3. Prop Density Rules

#### The Principle: Density = Psychological State

| Area Type | Density | Density Rationale | Count Guideline |
|----------|---------|----------------|--------------|
| **DayService (Safe)** | Sparse | Space, breath, the illusion of safety | 3-5 props per screen width. Each prop must be nameable ("that boat," "those nets," "the lantern"). |
| **ChoiceLock (Transitional)** | Medium | Tightening, the squeeze begins | 8-12 props per screen width. Props begin to overlap. |
| **NightSurvival (Danger)** | Dense | Clutter = obstacle = horror | 15-25 props per screen width. Nothing is random — density seeds hazards. |
| **Resolve (Victory)** | Sparse + Bare | Half-standing, half-gone | 3-5 props (half with burn holes). The other half's absence tells the story. |
| **Resolve (Defeat)** | Empty | Bare canvas is the story | 0 props. Bare paper. The absence itself is the prop. |

#### Specific Prop Rules by Category

**Functional Props (Player collision/interaction)**:
- Day: 1-2 per screen ("the lantern I must light," "the net I must climb")
- Night: Every functional prop becomes a *decision point* — use or avoid

**Atmospheric Props (Non-interactive)**:
- Day: Unlimited density if non-interactive (background texture)
- Night: Non-interactive props *become* interactive (hidden hazards in clutter)

**Narrative Props (Story without text)**:
- 1 per screen maximum
- Examples: A child's sandal beside a burned boat hull. A half-burned photograph held by charcoal. An altar with 3 bowls (Tú + 2 saved souls).

> **Rule**: Every prop must answer "who lived here?" If the answer is silence, the prop doesn't belong.

---

### 4. Environmental Storytelling

#### The Unspoken Narrative: What Details Tell the Story

**The Rules**:
1. **Never explain** — only imply
2. **Never force** — only reward observation
3. **Never mourn** — only show the absence of mourning

#### Story Beats Without Text

| Beat | Visual Detail | Placement | Why It Works |
|------|--------------|-----------|-------------|
| **Tú's Guilt** | Unfinished fishing net with tangled knots — the same net he couldn't cut to free the whale | Day: foreground, player passes within 2 units | He was a fisherman. He failed at the one thing fishermen do. |
| **The Abandoned Village** | Doors hanging open, no thresholds — everyone fled, no time to close | All phases, always visible | The absence of closed doors = the urgency of escape |
| **Partial Burn** | Structures with fresh char on one side, warm wood on other | Transition zones (ChoiceLock → Night boundary) | The fire came from one direction. It chose. |
| **Whale Presence** | Small carved whale totems, all facing south toward the player start | Night: hidden in dense areas, reward for observation | The whale god is always watching |
| **Sacrificed Soul Mark** | A pile of cloth (no body) with ember glow at edges | Night: where a soul was left behind | The location of sacrifice remains. The player sees where they failed. |
| **Three Empty Bowls** | Incense altar with one bowl intact, two knocked over | DayService: optional find, rewards exploration | Shows who's still alive. Count updates dynamically. |
| **The Shrinking Safe Zone** | Rope markers showing previous waterlines | Day: visible at ground level, fades by Night | The shade is actively retreating. Visual timer. |
| **Personal Effects** | One player's signature item visible per screen | Rotating spawn, one per zone | Rewards exploration. Answers "who was I saving?" |

#### The Master Rule: No Text, No Symbol, No UI Element

> **Test**: If a player can understand the story by looking at screenshots alone, the environmental storytelling works. If they need text on screen, we failed.

**The exception**: The Three Empty Bowls (count changes) can update — this is a game state indicator, not narrative. UI elements serve function; props serve story.

#### Outsourced Team Checklist for Environmental Storytelling

- [ ] Each screen has at least one prop that answers "who lived here?"
- [ ] Every sacrifice point has a corresponding ember mark visible in Night
- [ ] Density shifts match the phase (sparse → dense → empty)
- [ ] No prop exists that would look at home in a tourist brochure
- [ ] Player can understand "this place is dangerous now" from color alone
- [ ] Player can understand "someone died here" from layout alone
- [ ] No text, no symbols, no labels — pure visual grammar

---

### Section Summary: Deliverables for Outsourcing

| Deliverable | Format | Notes |
|------------|--------|-------|
| **Mood Board** | 5 reference images (Lý Sơn, Vietnamese fishing village, paper texture studies) | Not "make this" — "feel this" |
| **Prop Sheet** | 30-50 named props + density tier (S/M/D) | One sprite per prop, Day state only |
| **Shader Spec** | Unity Shader Graph or HLSL (1 shader, 2 states) | Painted → Charred transition is the technical deliverable |
| **Environmental Storytelling Brief** | One-pager of what each screen must convey | No images — written direction only |
| **Tile Palette** | Day/Night tile pairs, 2 variants each | Organic shapes (Day) + Geometric fragments (Night) |

*This section is complete whenoutsourcing can begin without verbal briefing.*

---

## Section 7: UI/HUD Visual Direction

### Hybrid Architecture: Diegetic + Screen-Space

| Element | Approach | Rationale |
|--------|---------|-----------|
| Health/Stamina bars | Diegetic | Part of body — feels like looking at your hands |
| Inventory/Item icons | Screen-space | Functional storage, needs clarity |
| Compass/Location | Diegetic | Physical object on player's back |
| Quest markers | Screen-space | External information |
| Damage indicators | Diegetic | Blood spatter/vignette — reactive |
| Dialogue text | Screen-space | Readability paramount |
| Pause menu | Screen-space | Meta-information |

### Diegetic Framing
- Player's body as anchor — health displays as watercolor heart on chest
- Ink-line veins radiate as damage accumulates
- Night: veins glow ember-orange at fractures

### Screen-Space Container
- Edges feel like torn paper scraps or burned canvas remnants
- Translucent — background bleeds through
- Floating at screen periphery, not full overlay

### Typography
- **Font**: Hand-painted aesthetic, uneven baseline, organic variation
- **Recommended**: Rosa Cleaner, Caveat, or custom with humanist hand-drawn qualities
- Hierarchy: H1 (2x body), H2 (1.5x), H3 (1.15x), Body (16-18px baseline), Caption (0.75x)

### Iconography: Outlined with Watercolour Fill
- Linework: Ink-brush strokes with variable width
- Fill: Watercolour wash inside lines — not solid
- Day: Near-black ink, transparent wash
- Night: Burnt umber, ember-orange internal glow

### Animation Feel
- **Core**: UI doesn't pop — it washes in like pigment diffusing
- Day: Soft, fluid, slightly slow — like underwater
- Night: Sharp, quick snaps, ember trails
- Timing: Panel open 350ms ease-out, close 250ms ease-in, hover 200ms

### UI Never-Use Colors
| Condition | Never | Use Instead |
|-----------|-------|------------|
| Day text on light | Pure black | Near-black (#1a1a1a) |
| Day background | Pure white | Parchment cream (#f5f0e6) |
| Night text on dark | Pure white | Ember cream (#ffe4c4) |
| Night background | Pure black | Charcoal (#1a1815) |

---

## Section 8: Asset Standards

*See separate file: `design/art/asset-standards.md`*

### Quick Reference

| Category | Standard |
|----------|----------|
| Textures | PNG with paper grain, no JPEG |
| Naming | PascalCase classes, _camelCase private |
| Resolution Tiers | Tier 1: 512px (far), Tier 2: 1024px (mid), Tier 3: 2048px (close) |
| Memory Budget | 256MB total texture |
| Day/Night Suffix | *_day.png, *_night.png |

---

## Section 9: Reference Direction

### Reference 1: Candle: The Power of the Flame (Game)
- **Draw from**: Visible paper texture, hand-painted imperfections, wet-on-wet blending
- **Avoid**: Flat cartoonish character design — we need muted, atmospheric

### Reference 2: Fred Williams — Burnt Landscape series (Artist)
- **Draw from**: Charred earth palette, contrast between blackened ground and warm remains
- **Avoid**: Photographic realism — we want organic watercolour softness

### Reference 3: Gris (Game)
- **Draw from**: Watercolour bleeding technique, emotional color shifts (warm ochres → cool violets), soft edge treatment
- **Avoid**: Flowing dress animations — we need weight and texture

### Reference 4: Hollow Knight (Game)
- **Draw from**: Biome-specific color palettes with dramatic contrast, lighting shifts between areas
- **Avoid**: Default to darkness — we embrace both day warmth AND night desolation

### Reference 5: John Sacret Young — "Forest Fire in Moonlight" (Watercolour, 1920)
- **Draw from**: Warm/cool contrast within single composition — white-hot interior rimmed with moonlit cool. Fire creates its own light.
- **Avoid**: Literal narrative content — use the color relationship, not the subject