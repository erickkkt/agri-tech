# Hướng dẫn gộp 3 repo vào monorepo `agri-tech`

Mục tiêu: gộp `farm-api`, `farm-admin`, `farm-user` vào repo
[`https://github.com/erickkkt/agri-tech.git`](https://github.com/erickkkt/agri-tech.git)
ở dạng các thư mục con.

Có 2 cách:

| Cách | Khi nào dùng | Lịch sử commit |
|------|--------------|----------------|
| **A. `git subtree`** (khuyến nghị) | Bạn muốn giữ lại toàn bộ commit history của 3 repo cũ | Giữ nguyên |
| **B. Copy fresh** | Bạn muốn bắt đầu sạch, không quan tâm history | Mất hết |

---

## ✅ Cách A: `git subtree` (giữ lịch sử)

### Bước 0: Push code mới nhất của 3 repo cũ lên remote

Trước khi merge, đảm bảo 3 repo cũ đã commit & push hết những thay đổi gần đây
(các Phase 1/2/3 component, popup, controller, v.v.).

```bash
cd farm-api && git add -A && git commit -m "Phase 1-3: backend complete" && git push
cd ../farm-admin && git add -A && git commit -m "Phase 1: admin UI complete" && git push
cd ../farm-user && git init && git add -A && git commit -m "Phase 2-3: user UI scaffold"
# (farm-user chưa từng được push - tạo repo mới trên GitHub rồi push hoặc bỏ qua bước này nếu sẽ subtree từ local path)
```

### Bước 1: Clone repo mới `agri-tech`

```bash
cd ~/projects                              # hoặc folder bạn muốn
git clone https://github.com/erickkkt/agri-tech.git
cd agri-tech
```

### Bước 2: Đưa các file root vào `agri-tech`

Copy các file đã chuẩn bị (README, .gitignore, docker-compose.yml,
agri-tech.code-workspace, docs/) từ folder `farm/` của bạn vào root của
`agri-tech/`:

```bash
# Giả sử bạn đang ở folder agri-tech, các file gốc ở /Users/vyvu3007/TaiHoang/project/farm/
cp /Users/vyvu3007/TaiHoang/project/farm/README.md .
cp /Users/vyvu3007/TaiHoang/project/farm/.gitignore .
cp /Users/vyvu3007/TaiHoang/project/farm/docker-compose.yml .
cp /Users/vyvu3007/TaiHoang/project/farm/agri-tech.code-workspace .
cp -r /Users/vyvu3007/TaiHoang/project/farm/docs .

git add .
git commit -m "chore: monorepo skeleton (README, gitignore, docker-compose, docs)"
git push
```

### Bước 3: Subtree-add từng repo

`git subtree add` sẽ kéo về toàn bộ lịch sử của mỗi repo và đặt vào thư mục
con với tiền tố (prefix). Lịch sử cũ vẫn truy cập được qua `git log`.

```bash
# Vẫn ở folder agri-tech/
git remote add farm-api-remote   https://github.com/<org>/farm-api.git
git remote add farm-admin-remote https://github.com/<org>/farm-admin.git
git remote add farm-user-remote  https://github.com/<org>/farm-user.git    # nếu có

# Subtree pull
git subtree add --prefix=farm-api    farm-api-remote   main --squash
git subtree add --prefix=farm-admin  farm-admin-remote main --squash
git subtree add --prefix=farm-user   farm-user-remote  main --squash
# Thay 'main' bằng 'master' / tên branch hiện tại của repo cũ nếu khác
# --squash gộp toàn bộ lịch sử cũ thành 1 commit duy nhất; bỏ flag này để giữ
# từng commit (lịch sử dài nhưng đầy đủ).
```

> **Lưu ý**: Nếu `farm-user` chưa có remote (chỉ là folder local), dùng cách
> sau thay cho `git remote add` + `subtree add`:
> ```bash
> mkdir farm-user
> cp -r /Users/vyvu3007/TaiHoang/project/farm/farm-user/* farm-user/
> git add farm-user && git commit -m "feat: import farm-user (no prior history)"
> ```

### Bước 4: Push monorepo

```bash
git push origin main
```

### Bước 5 (sau này): Sync ngược lại từng repo (nếu cần)

Nếu bạn muốn đẩy thay đổi trong monorepo về repo cũ:

```bash
git subtree push --prefix=farm-api    farm-api-remote   main
git subtree push --prefix=farm-admin  farm-admin-remote main
```

Tuy nhiên, sau khi đã chuyển sang monorepo thì bạn nên **archive 3 repo cũ**
trên GitHub (Settings → Archive this repository) và làm việc thẳng trên monorepo.

---

## 🆕 Cách B: Copy fresh (không giữ history)

Phù hợp nếu bạn không quan tâm lịch sử các repo cũ.

```bash
git clone https://github.com/erickkkt/agri-tech.git
cd agri-tech

# Copy 3 thư mục con (loại trừ .git của từng repo)
rsync -av --exclude='.git' /Users/vyvu3007/TaiHoang/project/farm/farm-api/   farm-api/
rsync -av --exclude='.git' /Users/vyvu3007/TaiHoang/project/farm/farm-admin/ farm-admin/
rsync -av --exclude='.git' /Users/vyvu3007/TaiHoang/project/farm/farm-user/  farm-user/

# Copy file root
cp /Users/vyvu3007/TaiHoang/project/farm/README.md .
cp /Users/vyvu3007/TaiHoang/project/farm/.gitignore .
cp /Users/vyvu3007/TaiHoang/project/farm/docker-compose.yml .
cp /Users/vyvu3007/TaiHoang/project/farm/agri-tech.code-workspace .
cp -r /Users/vyvu3007/TaiHoang/project/farm/docs .

git add .
git commit -m "feat: initial monorepo with farm-api, farm-admin, farm-user"
git push origin main
```

---

## Sau khi merge: cấu trúc cuối cùng

```
agri-tech/
├── .gitignore                    # tổng hợp .NET + Node
├── README.md                     # giới thiệu monorepo
├── docker-compose.yml            # orchestrate full stack
├── agri-tech.code-workspace      # multi-root workspace cho VS Code
├── docs/
│   ├── 00_CHANGELOG_v1.md
│   ├── 01_design_overview.docx
│   ├── 02_test_cases.xlsx
│   └── 03_MERGE_TO_MONOREPO.md   # file này
├── farm-api/                     # ASP.NET Core 8 backend
│   ├── Farm.sln
│   ├── Infrastructure/
│   ├── Services/
│   ├── Tests/
│   └── ...
├── farm-admin/                   # Angular admin app
│   ├── angular.json
│   ├── package.json
│   ├── src/
│   └── ...
└── farm-user/                    # Angular end-user app
    ├── README.md
    └── src/
```

## Tip - chạy nhanh

Sau khi gộp xong, mở VS Code bằng workspace file:

```bash
code agri-tech.code-workspace
```

VS Code sẽ hiển thị 5 folder gốc (monorepo, farm-api, farm-admin, farm-user, docs)
giúp navigate và search nhanh.

Để chạy full stack:

```bash
docker compose up --build
```

- Swagger:           http://localhost:8080/swagger
- Hangfire:          http://localhost:8080/hangfire
- SignalR hub:       http://localhost:8080/hubs/notifications
- Admin UI:          http://localhost:4200
- User UI:           http://localhost:4300

## Cleanup repo cũ (sau khi đã verify monorepo OK)

1. Vào GitHub mỗi repo cũ → **Settings** → cuối trang chọn **Archive this repository**.
2. Thông báo cho team: từ giờ làm việc trên `agri-tech` (cập nhật CI/CD,
   webhooks, integrations).
