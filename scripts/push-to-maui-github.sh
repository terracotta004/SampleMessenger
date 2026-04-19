#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
  echo "Usage: $0 <github-repo-url> [branch]"
  echo "Example: $0 git@github.com:your-user/MauiMessenger.git main"
  exit 1
fi

DEST_REPO_URL="$1"
DEST_BRANCH="${2:-main}"
DEST_REMOTE_NAME="maui"

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "Error: this script must be run inside a git repository."
  exit 1
fi

if git remote get-url "$DEST_REMOTE_NAME" >/dev/null 2>&1; then
  git remote set-url "$DEST_REMOTE_NAME" "$DEST_REPO_URL"
else
  git remote add "$DEST_REMOTE_NAME" "$DEST_REPO_URL"
fi

# Push all local branches and tags so the destination receives the complete history.
git push "$DEST_REMOTE_NAME" --all
git push "$DEST_REMOTE_NAME" --tags

# Ensure the requested default branch exists remotely.
git push "$DEST_REMOTE_NAME" "HEAD:refs/heads/$DEST_BRANCH"

echo
echo "Migration push complete."
echo "Destination remote '$DEST_REMOTE_NAME': $DEST_REPO_URL"
echo "Set default branch to '$DEST_BRANCH' in GitHub settings if needed."
