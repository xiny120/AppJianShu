SELECT ts_code as 股票代码,ts_name as 股票名称,powercalc20 as 20日量能 
FROM Stocks.PowerCalc 
where ts_market="主板" and market="SH"
order by powercalc20 desc;