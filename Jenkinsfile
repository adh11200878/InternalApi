pipeline {
    agent any

    environment {
        // SonarQube 在 Jenkins 中的名稱（在 Configure System 里設定）
        SONARQUBE_ENV = 'SonarQube'
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'master', url: 'https://github.com/adh11200878/InternalApi.git'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build InternalApi.sln'
            }
        }

        stage('SonarQube Analysis') {
            steps {
                withSonarQubeEnv("${SONARQUBE_ENV}") {
                    withCredentials([string(credentialsId: 'SonarQube', variable: 'SONAR_AUTH_TOKEN')]) {
                        bat """
                            REM 1. 開始 SonarQube 分析
                            dotnet sonarscanner begin /k:"InternalApi" /n:"InternalApi" /v:"1.0" /d:sonar.login=%SONAR_AUTH_TOKEN%

                            REM 2. 建置專案
                            dotnet build InternalApi.sln

                            REM 3. 結束 SonarQube 分析
                            dotnet sonarscanner end /d:sonar.login=%SONAR_AUTH_TOKEN%
                        """
                    }
                }
            }
        }

    post {
        success {
            echo 'Pipeline 成功完成！'
        }
        failure {
            echo 'Pipeline 執行失敗，請檢查錯誤訊息！'
        }
    }
}
