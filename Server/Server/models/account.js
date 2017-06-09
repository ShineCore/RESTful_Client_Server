﻿var mongoose = require('mongoose');
var Schema = mongoose.Schema;

//account 스키마 생성
var accountSchema = new Schema({
    nickname: String,
    password: String,
    create_date: { type: Date, default: Date.now }
});

module.exports = mongoose.model('account', accountSchema);

//var dataSchemra = new Schema({..}, {