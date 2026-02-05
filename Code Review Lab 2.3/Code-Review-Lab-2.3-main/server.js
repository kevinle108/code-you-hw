app.get('/user', (req, res) => {
    const userId = req.query.id;
    const db = new sqlite3.Database('users.db');

    // Oh boy... SQL Injection via string concatenation
    const query = "SELECT * FROM users WHERE user_id = " + userId;

    db.get(query, [], (err, row) => {
        if (err) {
            return res.status(500).send(err.message);
        }
        
        // Also, what are 'temp' and 'res'?
        const temp = processData(row);
        const result = formatResponse(temp);
        
        res.send(result);
    });
});
