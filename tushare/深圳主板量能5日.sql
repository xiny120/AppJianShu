SELECT ts_market,ts_code,ts_name,powercalc5 
FROM Stocks.PowerCalc 
where ts_market="主板" and market="SZ"
order by powercalc5 desc;