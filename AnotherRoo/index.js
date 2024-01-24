const { Console } = require("console");
const crypto = require("crypto");

const crashHash = "e2ab6519f205f613e5132abfba7e5820873c0c3a6982055e792cc3eab39d9898";
const salt = "0xd2867566759e9158bda9bf93b343bbd9aa02ce1e0c5bc2b37a2d70d391b04f14";

function crashPointFromHash(serverSeed) {
    console.log("Copied from the game: " + serverSeed);

    const hash = crypto
        .createHmac("sha256", serverSeed)
        .update(salt)
        .digest("hex");

    console.log("This is the hash that is used to get the crash: " + hash);

    const hs = parseInt(100 / 4);
    if (divisible(hash, hs)) {
        return 1;
    }


    const h = parseInt(hash.slice(0, 52 / 4), 16);
    console.log("BigInt: " + h);
    const power = Math.pow(2, 52);
    console.log("power: " + power);
    let crashPoint = Math.floor((100 * power - h) / (power - h)) / 100.0;
    console.log("crashPoint: " + crashPoint);




    return 1;
    // return Math.floor((100 * e - h) / (e - h)) / 100.0;
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

// this gets the previous hash
function generateGameHash(crashHash) {

    let gameHash = crypto.createHash("sha256").update(crashHash).digest("hex");
    console.log("gameHash:" + gameHash);
    return gameHash;
}


function test(){
    
    //var hashy = "2694c20d3fbca07f385284d5510ba3bf9bc9756969b6df98dd65560433d6a2dd";
    //var hashy = "6898b0e39e6b02d87bd614a1b7a32fd8280edc986eff0e5f0eda2a33087b809d";
    //var hashy = "018abaa1f5a0de5490977a5ce52a46df0e7ab3a1e1d564cca5c3e345c0efb383";
    //var hashy = "c0228fcfb955b8dd3da03126533723b926c902da460262a50c21cf12e9e8223c";
    var hashy = "ec2280a5f0b3100852097ca0aa81c5dc4a260cb062b7fb55e52a8b6febe8ac7c";
    var val = 0;

    var o = hashy.length % 4;
    console.log("hashy: " + hashy);
    console.log("hashy.length: " + hashy.length);
    const mod = parseInt(100 / 4);

    console.log("batch the characters in groups of 4s");
    for (var i = o > 0 ? o - 4 : 0; i < hashy.length; i += 4) {
        // console.log(val + " " + val << 16);
        // console.log(i + " " + val + " " + (val << 16) + " " + hashy.substring(i, i + 4) + " " + parseInt(hashy.substring(i, i + 4), 16) + " " + ((val << 16) + parseInt(hashy.substring(i, i + 4), 16)) % mod)
         val = ((val << 16) + parseInt(hashy.substring(i, i + 4), 16)) % mod;
         console.log(val);
    }

    console.log(val === 0);

}

//test();
crashPointFromHash(crashHash);
//generateGameHash(crashHash);