# HTTP IoT LAN Website Controller

## Install Node.js (macOS)

You need Node.js installed on your machine. Choose one option:

### Option A: Homebrew (recommended if you use Homebrew)

```bash
# Install Homebrew first if needed: https://brew.sh
brew install node
```

### Option B: nvm (Node Version Manager)

Run in your terminal (not inside Cursor’s sandbox):

```bash
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.1/install.sh | bash
```

Then restart your terminal or run:

```bash
source ~/.bashrc   # or source ~/.zshrc if you use zsh
nvm install --lts
nvm use --lts
```

### Option C: Official installer

Download the LTS installer from [nodejs.org](https://nodejs.org) and run it.

---

## After Node.js is installed

From this folder, install dependencies and run the project:

```bash
cd "HTTP-IoT-LAN-Website-Controller"
npm install
npm start
```

Check that Node is available:

```bash
node --version
npm --version
```
