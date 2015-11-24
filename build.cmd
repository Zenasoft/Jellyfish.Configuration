  

call dnvm use 1.0.0-rc1-final 
call dnu restore
if not "%errorlevel%"=="0" goto failure

IF '%Configuration%' == '' (
  call dnu pack src\Jellyfish.Configuration --configuration Release
  call dnu pack src\Jellyfish.Configuration.vnext --configuration Release
) ELSE (
  call dnu pack src\Jellyfish.Configuration --configuration %Configuration%
  call dnu pack src\Jellyfish.Configuration.vnext --configuration %Configuration%
)
if not "%errorlevel%"=="0" goto failure

cd test\Jellyfish.Configuration.test
call dnx test 
cd ../..
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1