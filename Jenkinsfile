pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        powershell(script: '.\\build.ps1', returnStdout: true)
      }
    }
    stage('Test') {
      steps {
        powershell(script: '.\\test.ps1', returnStdout: true)
      }
    }
  }
}