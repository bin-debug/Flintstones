const { Console } = require("console");
const crypto = require("crypto");

const gameHash = "cebb36c50d700e6a21f0ea92c47799af06522ce5db58fcee65dc83b748a70c20";
const salt = "0xd2867566759e9158bda9bf93b343bbd9aa02ce1e0c5bc2b37a2d70d391b04f14";

function getCrashPoint(){
    const hash = crypto
    .createHmac("sha256", gameHash)
    .update(salt)
    .digest("hex");

    console.log(hash);

    return hash;
} 

getCrashPoint();