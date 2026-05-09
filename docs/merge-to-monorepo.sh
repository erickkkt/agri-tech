#!/usr/bin/env bash
# ============================================================
# Agri-Tech monorepo - automated merge script
# ------------------------------------------------------------
# Usage:
#   ./merge-to-monorepo.sh                # Cách A (subtree, giữ lịch sử)
#   ./merge-to-monorepo.sh --fresh        # Cách B (copy fresh, mất lịch sử)
#
# Yêu cầu:
# - Đã clone agri-tech về máy
# - Đã commit & push code mới nhất của farm-api, farm-admin, farm-user
# - Biến môi trường (sửa trước khi chạy):
#     FARM_API_URL    = remote URL của farm-api repo
#     FARM_ADMIN_URL  = remote URL của farm-admin repo
#     FARM_USER_URL   = remote URL của farm-user repo (để trống nếu chưa có)
# ============================================================

set -euo pipefail

MONO_DIR="${MONO_DIR:-$(pwd)}"
SOURCE_DIR="${SOURCE_DIR:-/Users/vyvu3007/TaiHoang/project/farm}"
BRANCH="${BRANCH:-main}"

FARM_API_URL="${FARM_API_URL:-}"
FARM_ADMIN_URL="${FARM_ADMIN_URL:-}"
FARM_USER_URL="${FARM_USER_URL:-}"

MODE="subtree"
if [[ "${1:-}" == "--fresh" ]]; then
  MODE="fresh"
fi

echo "==> Monorepo dir : $MONO_DIR"
echo "==> Source dir   : $SOURCE_DIR"
echo "==> Mode         : $MODE"
echo "==> Branch       : $BRANCH"
echo

# ----- Sanity check -----
if [[ ! -d "$MONO_DIR/.git" ]]; then
  echo "ERROR: $MONO_DIR is not a git repo. Run 'git clone https://github.com/erickkkt/agri-tech.git' first." >&2
  exit 1
fi

cd "$MONO_DIR"

# ----- Step 1: copy root files -----
echo "==> Copying root files (README, .gitignore, docker-compose, workspace, docs)"
cp -f "$SOURCE_DIR/README.md" .
cp -f "$SOURCE_DIR/.gitignore" .
cp -f "$SOURCE_DIR/docker-compose.yml" .
cp -f "$SOURCE_DIR/agri-tech.code-workspace" .
mkdir -p docs
cp -r "$SOURCE_DIR/docs/." docs/

git add .
git commit -m "chore: monorepo skeleton" || echo "(nothing to commit at root)"

# ----- Step 2: import sub-projects -----
import_subtree() {
  local PREFIX="$1"
  local URL="$2"
  if [[ -z "$URL" ]]; then
    echo "==> SKIP $PREFIX (no remote URL provided)"
    return
  fi
  if [[ -d "$PREFIX" ]]; then
    echo "==> SKIP $PREFIX (folder already exists in monorepo)"
    return
  fi
  echo "==> subtree add $PREFIX <- $URL@$BRANCH"
  git remote add "${PREFIX}-remote" "$URL" 2>/dev/null || true
  git fetch "${PREFIX}-remote" "$BRANCH"
  git subtree add --prefix="$PREFIX" "${PREFIX}-remote" "$BRANCH" --squash
}

import_fresh() {
  local PREFIX="$1"
  local SRC="$SOURCE_DIR/$PREFIX/"
  if [[ ! -d "$SRC" ]]; then
    echo "==> SKIP $PREFIX (source folder not found at $SRC)"
    return
  fi
  if [[ -d "$PREFIX" ]]; then
    echo "==> SKIP $PREFIX (folder already exists in monorepo)"
    return
  fi
  echo "==> rsync $PREFIX from $SRC"
  rsync -av --exclude='.git' --exclude='node_modules' --exclude='bin' --exclude='obj' --exclude='dist' "$SRC" "$PREFIX/"
  git add "$PREFIX"
  git commit -m "feat: import $PREFIX into monorepo" || true
}

if [[ "$MODE" == "subtree" ]]; then
  import_subtree "farm-api"   "$FARM_API_URL"
  import_subtree "farm-admin" "$FARM_ADMIN_URL"
  import_subtree "farm-user"  "$FARM_USER_URL"

  # If farm-user has no remote, fall back to copying from local
  if [[ -z "$FARM_USER_URL" && ! -d "farm-user" ]]; then
    echo "==> farm-user has no remote URL - falling back to local copy"
    import_fresh "farm-user"
  fi
else
  import_fresh "farm-api"
  import_fresh "farm-admin"
  import_fresh "farm-user"
fi

echo
echo "==> Done. Review with 'git log' then push:"
echo "    git push origin $BRANCH"
