  
CMD /C dnu restore
if NOT ERRORLEVEL 0 EXIT /B 1

IF '%Configuration%' == '' (
  CMD /C dnu pack src\Jellyfish.Configuration --configuration Release
) ELSE (
  CMD /C dnu pack src\Jellyfish.Configuration --configuration %Configuration%
)
if NOT ERRORLEVEL 0 EXIT /B 1

cd test\Jellyfish.Configuration.test
CMD /C dnx test 

cd ..\..

if NOT ERRORLEVEL 0 EXIT /B 1