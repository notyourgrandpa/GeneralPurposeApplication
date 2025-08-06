### 📘 `FAQ.md` (Frequently Asked Questions)
```markdown
# ❓ FAQ (Frequently Asked Questions)
---

### Q: The app doesn't start or crashes at launch.

Make sure:
- You have Node/.NET installed correctly.
- You ran `npm install`.
- You installed all listed dependencies on the repository page.

---

### Q: Where is your appsettings.json located?

I'm using secrets.json for sensitive data, so you won't find appsettings.json in the repository. You can create your own appsettings.json file in the root directory of the project server.

Example appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SampleApp;" "User Id=SampleUser;Password=SamplePassword; Integrated Security=False;MultipleActiveResultSets=True;TrustServerCertificate=True"
  },
  "DefaultPasswords": {
    "RegisteredUser": "Sampl3Pa$$_User",
    "Administrator": "Sampl3Pa$$_Admin"
  }
}
```
