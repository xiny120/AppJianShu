SELECT ts_market,ts_code,ts_name,powercalc20 
FROM Stocks.PowerCalc 
where ts_market="主板" and market="SZ"
order by powercalc20 desc;