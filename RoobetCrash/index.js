// Client Seed: hash of ETH block 17985520 (0xd2867566759e9158bda9bf93b343bbd9aa02ce1e0c5bc2b37a2d70d391b04f14)
// 2 millionth server seed: 76a11ece9e181cb5f0a71e71c45ca0e431b904306b9cb161fd43025a5d3ebb8d
// Public Seed Event: https://pastebin.com/1cGDjFyv

const { Console } = require("console");
const crypto = require("crypto");

const crashHash = "023114701a161dd07beca6de2f43913648e706af628cfc3f457fb4e6fd5ea2bc";
const salt = "0xd2867566759e9158bda9bf93b343bbd9aa02ce1e0c5bc2b37a2d70d391b04f14";

function generateHash(seed) {

    // let s = crypto.createHash("sha256").update(seed).digest("hex");
    // console.log("some jazz" + s);

    return crypto.createHash("sha256").update(seed).digest("hex");
}

function divisible(hash, mod) {
    // We will read in 4 hex at a time, but the first chunk might be a bit smaller
    // So ABCDEFGHIJ should be chunked like  AB CDEF GHIJ
    var val = 0;
  
    var o = hash.length % 4;
    for (var i = o > 0 ? o - 4 : 0; i < hash.length; i += 4) {
      val = ((val << 16) + parseInt(hash.substring(i, i + 4), 16)) % mod;
    }
  
    return val === 0;
  }

function crashPointFromHash(serverSeed) {
    const hash = crypto
        .createHmac("sha256", serverSeed)
        .update(salt)
        .digest("hex");


    console.log(hash);

    const hs = parseInt(100 / 4); // house edge
    if (divisible(hash, hs)) {
        console.log(true);
        return 1;
    }

    const h = parseInt(hash.slice(0, 52 / 4), 16);
    const e = Math.pow(2, 52);

    return Math.floor((100 * e - h) / (e - h)) / 100.0;
}

function getPreviousGames() {
    const previousGames = [];
    let gameHash = generateHash(crashHash);

    for (let i = 0; i < 11; i++) {
        const gameResult = crashPointFromHash(gameHash);

        previousGames.push({ gameHash, gameResult });
        gameHash = generateHash(gameHash);
        
    }

    return previousGames;
}

function verifyCrash() {
    const gameResult = crashPointFromHash(crashHash);
    const previousHundredGames = getPreviousGames();

    return { gameResult, previousHundredGames };
}

// function verifyCrash() {
//     const gameResult = crashPointFromHash(crashHash);
//     return { gameResult };
// }


console.log(verifyCrash());
