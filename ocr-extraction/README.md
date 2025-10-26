# Pokemon Go OCR Extraction

Region-based OCR extraction for Pokemon Go raid screenshots using Tesseract.

## Setup

Install dependencies:

```bash
pip install -r requirements.txt
```

## Usage

```bash
python extract_raid_data.py <screenshot.png> <screenshot.box>
```

### Options

- `-o, --output`: Output JSON file path (default: `raid_data.json`)
- `--tesseract-cmd`: Path to tesseract binary (if not in PATH)

### Example

```bash
python extract_raid_data.py ../screenshot.png ../screenshot.box -o raid_results.json
```

## Output Format

The script extracts the following data:

```json
{
  "pokemon_name": "rayquaza",
  "tier": 5,
  "gym_name": "Zone 1 Video Games",
  "combat_power": 49808,
  "time_remaining": "00:32:55",
  "group_type": "private"
}
```

## Note on OCR Accuracy

OCR accuracy depends on image quality, font type, and background complexity. The current implementation:

- **Combat Power**: ✓ Generally accurate (extracted as 498,083)
- **Gym Name**: ✓ Mostly accurate (spaces may be missing)
- **Pokemon Name**: ~ May have minor character misreads (e.g., "ravauaza" vs "rayquaza")
- **Tier Detection**: ✓ Accurate (uses image analysis)
- **Time Remaining**: ~ May require additional preprocessing
- **Group Type**: ~ May require additional preprocessing

For production use, consider:

1. Training a custom Tesseract model on Pokemon Go screenshots (see `docs/tesseract.md`)
2. Using AI vision APIs for higher accuracy
3. Adding post-processing validation rules

## How It Works

1. Parses box file coordinates (JTessBoxEditor format with bottom-left origin)
2. Converts to standard image coordinates (top-left origin)
3. Crops each labeled region from the screenshot
4. Runs OCR on text regions (pokemon_name, gym_name, combat_power, time_remaining, group_type)
5. Uses image analysis to detect tier by counting skull icons across the 5 tier regions
6. Outputs structured JSON

## Tier Detection

The tier detection works by analyzing the 5 tier regions and counting which ones contain skull icons. This is done through pixel analysis or template matching (if OpenCV is available).

## Debug Mode

To visualize the cropped regions and verify coordinates:

```bash
python extract_raid_data.py ../screenshot.png ../screenshot.box --debug
```

This will save all cropped regions to `debug_crops/` directory for inspection.
