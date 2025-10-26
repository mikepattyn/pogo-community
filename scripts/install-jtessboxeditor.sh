#!/bin/bash
# Script to install jTessBoxEditor on macOS

set -e

echo "🔧 Installing jTessBoxEditor for macOS..."

# Configuration
JTESSBOXEDITOR_VERSION="2.7.0"
DOWNLOAD_URL="https://github.com/nguyenq/jTessBoxEditor/releases/download/Release-${JTESSBOXEDITOR_VERSION}/jTessBoxEditor-${JTESSBOXEDITOR_VERSION}.zip"
INSTALL_DIR="$HOME/.local/jTessBoxEditor"
BINARY_NAME="jTessBoxEditor.jar"

# Create installation directory
echo "📁 Creating installation directory..."
mkdir -p "$INSTALL_DIR"

# Download jTessBoxEditor
echo "⬇️  Downloading jTessBoxEditor v${JTESSBOXEDITOR_VERSION}..."
cd /tmp
curl -L -o jTessBoxEditor.zip "$DOWNLOAD_URL"

# Extract the downloaded file
echo "📦 Extracting files..."
unzip -q jTessBoxEditor.zip -d "$INSTALL_DIR"

# Remove the zip file
rm jTessBoxEditor.zip

# Find the jar file
JAR_FILE=$(find "$INSTALL_DIR" -name "$BINARY_NAME" -type f | head -n 1)

if [ -z "$JAR_FILE" ]; then
    echo "❌ Error: Could not find jTessBoxEditor.jar"
    exit 1
fi

echo "✅ Found jTessBoxEditor.jar at: $JAR_FILE"

# Create a launcher script
echo "📝 Creating launcher script..."
LAUNCHER="$INSTALL_DIR/jtessboxeditor"
cat > "$LAUNCHER" <<EOF
#!/bin/bash
exec java -jar "$JAR_FILE" "\$@"
EOF

chmod +x "$LAUNCHER"

# Add to PATH (optional - create symlink in /usr/local/bin if desired)
if [ -w "/usr/local/bin" ]; then
    echo "🔗 Creating symlink in /usr/local/bin..."
    ln -sf "$LAUNCHER" /usr/local/bin/jtessboxeditor
    echo "✅ jTessBoxEditor is now available as 'jtessboxeditor'"
else
    echo "ℹ️  Run this to use jTessBoxEditor:"
    echo "   $INSTALL_DIR/jtessboxeditor"
    echo ""
    echo "Or add this to your ~/.zshrc:"
    echo "   export PATH=\"\$PATH:$INSTALL_DIR\""
fi

echo ""
echo "✅ jTessBoxEditor installed successfully!"
echo "📍 Installation directory: $INSTALL_DIR"
echo ""
echo "🚀 To run jTessBoxEditor:"
if [ -w "/usr/local/bin" ]; then
    echo "   jtessboxeditor"
else
    echo "   $LAUNCHER"
fi

