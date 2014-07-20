# N Day 

A structured import utility for creating Day One entries from text files.

## How it works

nday is intended to work in batch mode.  Every now and then, invoke it 
and it will look for the file passed as parameter.

Usage:  `nday datafile`

It only expects a single file, since it is able to work out where the file contents
came from.

Entries should follow this format:

`(origin)(date)(activity)(tags)`

one entry per line.  Any lines that do not match this pattern are ignored.

Once the file has been read and the entries created in dayone, the file will be renamed to show when the import was done. So if your file is called `activity.txt` then it will be renamed to `activity.txt-imported20140718.txt`.
