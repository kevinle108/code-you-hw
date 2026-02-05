# User Notes:
The output style is different than Claude. It didn't break down and show each individual code snippet but rather 1 giant snippet in the end.
---
User: I want you to act as a Senior Python Developer. Here are examples of the feedback style I expect:
Example 1: 'On line 12, instead of manually opening a connection, consider using a context manager (with) to ensure the DB connection closes automatically.'
Example 2: 'On line 19, the string concatenation is a SQL injection vulnerability. Use parameterized queries: cursor.execute("... WHERE id = ?", (user_id,)).'
Example 3: 'On line 24, the variable name temp is vague. Rename it to user_profile to match the data it holds.'Now, audit this code: [Paste Python Code]"

GitHub Copilot: Summary — main issues found
- Line 24–25: SQL injection via string concatenation.
- Line 19: request.args.get('id') is unchecked and may be None or non-numeric.
- Lines 11–14: DB connection creation has no row factory and no teardown to close it.
- Lines 29–31: vague variable names (temp, res).

Line-by-line actionable feedback (style like your examples)
- On line 12, instead of storing the raw connection only, set a row factory (sqlite3.Row) so query results are easier to work with and consistent.
- On lines 11–14, register a teardown function to close g.db at the end of the request (use app.teardown_appcontext) so connections are not leaked.
- On line 19, validate and coerce the id: use request.args.get('id', type=int) or cast to int with a try/except and return 400 when missing/invalid.
- On line 24–25, the string concatenation is a SQL injection vulnerability. Use parameterized queries: cursor.execute("SELECT * FROM users WHERE user_id = ?", (user_id,))
- On line 27, check fetchone() result for None and return 404 if not found.
- On lines 29–31, the variable names temp and res are vague. Rename to user_profile and response (or similarly descriptive names).
- Also consider storing the DB path in the instance folder (os.path.join(app.instance_path, 'users.db')) and creating the folder with os.makedirs(..., exist_ok=True) for predictable deployments.
- For anything beyond simple CRUD, consider using an ORM (SQLAlchemy) to avoid manual SQL issues.

Suggested code changes
- Replaces get_db, adds teardown, and updates get_user_profile to validate input, use parameterized queries, and use better names.

````python
# ...existing code...
app = Flask(__name__, instance_relative_config=True)
os.makedirs(app.instance_path, exist_ok=True)
app.config.from_mapping(DATABASE=os.path.join(app.instance_path, 'users.db'))
# ...existing code...

def get_db():
    if 'db' not in g:
        g.db = sqlite3.connect(app.config['DATABASE'], detect_types=sqlite3.PARSE_DECLTYPES)
        g.db.row_factory = sqlite3.Row
    return g.db

@app.teardown_appcontext
def close_db(exc):
    db = g.pop('db', None)
    if db is not None:
        db.close()
# ...existing code...

@app.route('/user')
def get_user_profile():
    user_id = request.args.get('id', type=int)
    if user_id is None:
        return "Missing or invalid 'id' parameter", 400

    db = get_db()
    cursor = db.execute("SELECT * FROM users WHERE user_id = ?", (user_id,))
    user_data = cursor.fetchone()
    if user_data is None:
        return "User not found", 404

    user_profile = process_data(user_data)
    response = format_response(user_profile)
    return response
# ...existing code...
````

If you want, I can open this file and apply the patch for you and/or add a small unit test that verifies invalid/missing id returns 400 and that SQL injection attempts don't execute.