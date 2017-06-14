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
    var account = new Account();
    account.nickname = req.body.nickname;
    account.password = req.body.password;
    account.create_date = Date.now();
    account.save(function (err) {
        if (err) {
            console.error(err);
            res.json({ result: 0 });
            return;
        }
        res.json({ result: 1 });
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
//유저 정보 변경 - 찾아서 변경
//app.put('/api/accounts/:nickname', function (req, res) {
//    Account.findOne({ nickname: req.params.nickname }, function (err, account) {
//        if (err) {
//            return res.status(500).json({ error: 'database failure' });
//        }
//        if (!account) {
//            return res.status(404).json({ error: 'account not found' });
//        }
//        if (req.body.password) account.password = req.body.password;
//        if (req.body.create_date) account.create_date = req.body.create_date;
//        account.save(function (err) {
//            if (err) {
//                res.status(500).json({error: 'failed to update'});
//            }
//            res.json({message: 'change password'});
//        });
//    });
//});
//유저 정보 변경 - update사용
app.put('/api/accounts/:nickname', function (req, res) {
    Account.update({ nickname: req.params.nickname }, { $set: req.body }, function (err, output) {
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
app.delete('/api/accounts/:nickname', function (req, res) {
    Account.remove({ nickname: req.body.nickname }, function (err, output) {
        if (err) {
            return res.status(500).json({ error: 'database failure' });
        }
        res.status(204).end();
    });
});
//# sourceMappingURL=app.js.map