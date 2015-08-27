  
call dnu restore

IF '%Configuration%' == '' (
  call dnu pack src\Jellyfish.Configuration --configuration Release
) ELSE (
  call dnu pack src\Jellyfish.Configuration --configuration %Configuration%
)

cd test\Jellyfish.Configuration.test
call dnx test 

cd ..\..