# totp-net

## Origin

Project based on: https://github.com/mirthas/totp-net

## What is

A little totp c# library + CLI project.

## Quickstart

Generate the Actual code for the Secret GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ.
The secret has to be base32 encoded.
In this Example t1 is set to 30sec and we will get a 8 digit long code.

```c#
string secret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";
Totp totp = new Totp(secret,30,8);
string totpCode = totp.getCodeString();
```

## Resorces

* [rfc6238](https://tools.ietf.org/html/rfc6238)

## Used Librarys and Classes

* [Base32](http://scottless.com/blog/archive/2014/02/15/base32-encoder-and-decoder-in-c.aspx)
