node {
	stage 'Checkout'
		checkout scm

	stage 'Build'
		bat 'nuget restore ZetaTelnet.sln'
		bat "\"${tool 'MSBuild'}\" ZetaTelnet.sln /p:Configuration=Release /p:Platform=\"Any CPU\" /p:ProductVersion=1.0.0.${env.BUILD_NUMBER}"

	stage 'Archive'
		archive 'ZetaTelnet/bin/Release/**'

}
