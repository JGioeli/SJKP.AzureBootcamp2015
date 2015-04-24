--- Example of a SQL that selects all tweets with a specific language (48 = english)
SELECT
    *
INTO
    tweetsblob
FROM
    tweethubsimple
Where lang = 48 

