# 🛠️ Troubleshooting

Common errors and how to fix them.

---

### ❌ Error: Microsoft.Data.SqlClient.SqlException: 'A connection was successfully established with the server, but then an error occurred during the login process. (provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.)'

**Cause:** Security property of SQL Server set to Windows Authentication mode.  
**Fix:** Open SQL Server Management Studio, right-click on the server, select Properties, go to Security, and change the Server Authentication to SQL Server and Windows Authentication mode. Restart the SQL Server service after making this change.

---