pipeline {
  agent any
  stages {
    stage('build') {
      steps {
        sh '''
local_path=`echo ${PWD/var/home}`
local_path=`echo ${local_path/jenkins_home/"docker/jenkins"}`
echo "$local_path"
REAL_PATH="$local_path"
echo "$REAL_PATH"
docker run --rm -v "$REAL_PATH":/app -w /app mcr.microsoft.com/dotnet/core/sdk:2.1 dotnet restore
docker run --rm -v "$REAL_PATH":/app -w /app mcr.microsoft.com/dotnet/core/sdk:2.1 dotnet publish -c Release -o ./obj/Docker/publish
docker build -t "$REGISTRY_URL"/"$REGISTRY_IMAGE" .
docker login -u "$DOCKER_USERNAME" -p "$DOCKER_PASSWORD" "$REGISTRY_URL" && docker push "$REGISTRY_URL"/"$REGISTRY_IMAGE"'''
      }
    }

  }
  environment {
    REGISTRY_URL = 'registry.cn-qingdao.aliyuncs.com'
    REGISTRY_IMAGE = 'a-cubic/bonus-gift-server'
    DOCKER_USERNAME_USR = 'credentials(\'Docker_Push\')'
    DOCKER_PASSWORD_PSW = 'credentials(\'Docker_Push\')'
    CACHE = 'bonus-gift-server'
  }
}
