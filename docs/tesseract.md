# Tesseract OCR Training for Pokemon Go Screenshots

This guide documents the process of training a custom Tesseract OCR model to read text from Pokemon Go gym screenshots.

## 📋 Overview

We trained a custom Tesseract model (`screenshot.traineddata`) specifically designed to recognize text from gym screenshots with the following elements:

- Tier labels (One, Two, Three, Four, Five)
- Gym name labels
- Combat Power (CP) labels
- Pokemon name labels
- Time remaining labels
- Group type labels

## 📦 Prerequisites

- Tesseract OCR installed via Homebrew
- Training tools for Tesseract
- Python (optional, for automation)

## 🔍 Installation Location

On macOS with Homebrew (Apple Silicon):

```
tessdata location: /opt/homebrew/Cellar/tesseract/5.5.1/share/tessdata/
```

To copy the trained model:

```bash
sudo cp screenshot.traineddata /opt/homebrew/Cellar/tesseract/5.5.1/share/tessdata/
```

## 📝 Training Process

### 1. Prepare Training Data

Created box files for the screenshot showing the positions and text of various UI elements:

```11:11:screenshot.box
tier_label_one 350 2006 425 2081 0
tier_label_two 435 2006 510 2081 0
tier_label_three 525 2006 600 2081 0
tier_label_four 610 2006 685 2081 0
tier_label_five 698 2006 773 2081 0
gym_name_label 270 2181 870 2256 0
combat_power_label 200 1816 950 1986 0
pokemon_name_label 200 1666 950 1816 0
time_remaining_label 790 916 1090 1066 0
group_type_label 200 456 930 656 0
```

### 2. Font Properties

Created font properties file:

```2:2:screenshot.font_properties
screenshot 0 0 0 0 0
```

### 3. Training Commands

The training process involved:

```bash
# Make box file from TIF image
tesseract screenshot.tif screenshot batch.nochop makebox

# Train the model
bash tesstrain.sh --fonts_dir /fonts --fontlist "screenshot" \
  --lang screenshot \
  --linedata_only --langdata_dir . \
  --training_text screenshot.training_text \
  --output_dir screenshot_output \
  --maxpages 100

# Combine the training data
combine_tessdata -e screenshot_output/screenshot.lstmf \
  screenshot_output/screenshot.traineddata
```

### 4. Generated Files

- `screenshot.traineddata` - The trained model (219KB)
- `screenshot.box` - Box file with character coordinates
- `screenshot.font_properties` - Font characteristics
- `screenshot.tr` - Training data
- `training.log` - Training log output

## 🎯 Usage

After installing the model, use it with:

```bash
tesseract screenshot.tif output -l screenshot
```

## 📁 Training Artifacts

The following files were created during the training process:

### Required Files:

- `screenshot.traineddata` - Final trained model ✓
- `screenshot.box` - Character bounding boxes
- `screenshot.tr` - Training data
- `screenshot.font_properties` - Font properties

### Intermediate Files:

- `screenshot.png` - Source image (PNG format)
- `screenshot.tif` - Source image (TIF format)
- `screenshot.inttemp`, `screenshot.normproto`, `screenshot.pffmtable`, etc. - Intermediate training files
- `training.log` - Training output logs

### Word Lists:

- `en.words_list` - Word frequency list
- `en.frequent_words_list` - Common words
- `en.font_properties` - English font properties

## 🧹 Cleanup

After successful training, you can remove intermediate files:

```bash
# Remove intermediate training files
rm screenshot.inttemp screenshot.normproto screenshot.pffmtable \
   screenshot.shapetable screenshot.unicharset \
   screenshot.tr screenshot.box screenshot.font_properties \
   en.words_list en.frequent_words_list en.font_properties

# Keep only the trained model
rm screenshot.png screenshot.tif screenshot.box
```

## 📚 References

- [Tesseract Training Tutorial](https://tesseract-ocr.github.io/tessdoc/tess3/TrainingTesseract3.html)
- [Tesseract GitHub Repository](https://github.com/tesseract-ocr/tesseract)
- [Tesstrain Script](https://github.com/tesseract-ocr/tesstrain)

## 🎮 Pokemon Go Specific

This model is optimized for reading Pokemon Go gym screenshot text including:

- **Tier Labels**: Recognition of "tier_label_one" through "tier_label_five"
- **Gym Information**: Gym names, combat power values, Pokemon names
- **Time Information**: Raid timers and time remaining
- **Group Types**: Raid boss type identification

## 🚀 Next Steps

To improve accuracy:

1. Add more training examples with different gym screenshots
2. Train on multiple variations of font sizes and positions
3. Include examples with different lighting and background colors
4. Fine-tune the box file positions for better character detection

## 💡 Tips

- Always create high-quality box files by carefully positioning bounding boxes
- Use consistent font properties across training samples
- Train on diverse examples to improve generalization
- Test the model on real screenshots to verify accuracy

## 📊 Training Summary & Best Practices

### Understanding Your Current Setup

You have a `.box` file with **custom labels** (region identifiers) rather than actual text characters:

```plaintext
tier_label_one 350 2006 425 2081 0
gym_name_label 270 2181 870 2256 0
combat_power_label 200 1816 950 1986 0
```

These labels define **UI regions** where text appears, not the text itself.

### 🎯 Two Approaches to OCR

#### Option 1: Region-Based Extraction (Recommended for Custom Labels)

Since your `.box` file contains region identifiers (not actual text), train Tesseract to recognize these labels:

1. **Manual extraction** - Crop each region and run OCR individually:

```bash
# Example: Extract tier_label_one region
convert screenshot.png -crop 75x75+350+2006 tier1_crop.png
tesseract tier1_crop.png tier1_output
```

2. **Use existing OCR** - Apply to entire image with custom trained model:

```bash
tesseract screenshot.png output -l screenshot
```

#### Option 2: Full Training Pipeline (For Actual Text)

If training with real characters:

```bash
# Step 1: Extract unicharset
unicharset_extractor screenshot.tr

# Step 2: Run training tools
mftraining -F screenshot.font_properties -U unicharset -O screenshot.unicharset screenshot.tr
cntraining screenshot.tr

# Step 3: Shape clustering
shapeclustering -F screenshot.font_properties -U unicharset screenshot.tr

# Step 4: Rename outputs
mv shapetable screenshot.shapetable
mv pffmtable screenshot.pffmtable
mv normproto screenshot.normproto
mv inttemp screenshot.inttemp
mv unicharset screenshot.unicharset

# Step 5: Combine into traineddata
combine_tessdata screenshot.
```

### 🛠️ Common Issues & Solutions

#### Issue: "command not found: tesseract"

**Solution**: Add tesseract to your PATH or use full path:

```bash
# Apple Silicon
/opt/homebrew/bin/tesseract

# Intel Mac
/usr/local/bin/tesseract
```

#### Issue: "command not found: shapeclustering"

**Solution**: Install training tools:

```bash
brew install tesseract --HEAD
# OR locate the tool:
find /opt/homebrew /usr/local -name "shapeclustering" 2>/dev/null
```

#### Issue: "cannot run program /usr/bin/tesseract"

**Solution**: Create symlink to expected location:

```bash
sudo mkdir -p /usr/bin
sudo ln -sf $(which tesseract) /usr/bin/tesseract
```

### 📝 File Structure Reference

```plaintext
pogo/
├── screenshot.png          # Source image
├── screenshot.tif          # Training image (TIF format)
├── screenshot.box          # Bounding box annotations
├── screenshot.tr           # Training data
├── screenshot.font_properties  # Font characteristics
├── output.txt              # OCR output
└── training.log            # Training process log
```

### ⚡ Quick Reference Commands

```bash
# Basic OCR extraction
tesseract screenshot.png output

# OCR with custom traineddata
tesseract screenshot.png output -l screenshot

# Create box file from image
tesseract screenshot.tif screenshot batch.nochop makebox

# Generate training data from box
tesseract screenshot.tif screenshot -l eng nobatch box.train

# Find tesseract location
which tesseract

# List available language data
tesseract --list-langs

# Check tessdata directory
echo $(dirname $(dirname $(which tesseract)))/share/tessdata
```

---

**Note**: The training process creates various intermediate files that can be cleaned up after successful training. Always keep the `.traineddata` file as it contains the final model.
