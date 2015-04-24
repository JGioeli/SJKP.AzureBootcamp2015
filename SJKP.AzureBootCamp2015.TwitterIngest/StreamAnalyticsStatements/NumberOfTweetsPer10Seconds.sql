--- Example of a SQL the returns the number of tweets within a 10 second window
SELECT
    System.TimeStamp as T1,  Count(*), Datepart(yy,System.TimeStamp) as P1
INTO
    TweetCountTable
FROM
    tweethubsimple
    Timestamp by Created_At
Where lang = 48 
GROUP BY TumblingWindow(second,10)
