var express = require('express');
var app = express();
var bodyParser = require('body-parser');
var mongoose = require('mongoose');
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
//var port = process.env.PORT || 8080;
var port = 8080;
var server = app.listen(port, function () {
    console.log("Express server has started on port" + port);
});
var db = mongoose.connection;
db.on('error', console.error);
db.once('open', function () {
    console.log("Connected to mongod server");
});
mongoose.connect('mongodb://localhost/GameDB');
var Account = require('./models/account');
var router = require('./routes')(app, Account);
//유저 Account 생성
app.post('/api/accounts', function (req, res) {
    Account.findOne({ id: req.body.id }, function (err, account) {
        if (err) {
            return res.status(500).json({ error: err });
        }
        //계정 없으면 생성
        if (!account) {
            var newaccount = new Account();
            newaccount.id = req.body.id;
            newaccount.nickname = req.body.nickname;
            newaccount.create_date = Date.now();
            newaccount.save(function (err) {
                if (err) {
                    console.error(err);
                    res.json({ result: 0 });
                    return;
                }
                res.json({ result: 1 });
            });
        }
        else {
            return res.status(400).json({ error: 'this account already exists' });
        }
    });
});
//유저 Account 전체 조회
app.get('/api/accounts', function (req, res) {
    Account.find(function (err, accounts) {
        if (err) {
            return res.status(500).send({ error: 'database failure' });
        }
        res.json(accounts);
    });
});
//유저 이름으로 조회
app.get('/api/accounts/:nickname', function (req, res) {
    Account.findOne({ nickname: req.params.nickname }, function (err, account) {
        if (err) {
            return res.status(500).json({ error: err });
        }
        if (!account) {
            return res.status(404).json({ error: 'account not found' });
        }
        res.json(account);
    });
});
//유저 정보 변경 - update사용
app.put('/api/accounts/:id', function (req, res) {
    Account.update({ id: req.params.id }, { $set: req.body }, function (err, output) {
        if (err) {
            res.status(500).json({ error: 'database failure' });
            console.log(output);
        }
        if (!output.n) {
            return res.status(404).json({ error: 'accout not found' });
        }
        res.json({ message: 'account update' });
    });
});
//유저 삭제
app.delete('/api/accounts/:id', function (req, res) {
    Account.remove({ id: req.params.id }, function (err, output) {
        if (err) {
            return res.status(500).json({ error: 'database failure' });
        }
        res.json({ message: 'account delete' });
        res.status(204).end();
    });
});
//# sourceMappingURL=app.js.map