# Security Policy

## Supported Versions

Use this section to tell people about which versions of your project are
currently being supported with security updates.



This guide shows you how to configure security features for a repository.
You must be a repository administrator or organization owner to configure security settings this repository.

The first step to securing a repository is to set up who can see and modify your code.

From the main page of your repository, click Settings, then scroll down to the "Danger Zone."

To change who can view your repository, click Change visibility. 
To change who can access your repository and adjust permissions, click Manage access. 

The dependency graph is automatically generated for all public repositories and you can choose to enable it for private repositories.


code checks completed via SonarCloud
Dependabot alerts all enabled.
To enable Dependabot version updates, you must create a dependabot.yml configuration file.
Dependabot security updates all enabled.


| Version | Supported          |
| ------- | ------------------ |
| 5.1.x   | :white_check_mark: |
| 5.0.x   | :x:                |
| 4.0.x   | :white_check_mark: |
| < 4.0   | :x:                |

## Reporting a Vulnerability

Use this section to tell people how to report a vulnerability.

Tell them where to go, how often they can expect to get an update on a
reported vulnerability, what to expect if the vulnerability is accepted or
declined, etc.


I may potentially need to add a darker file under github folder the file contains the following...

FROM sonarsource/sonar-scanner-cli:4.6

LABEL version="0.0.1" \
      repository="https://github.com/umgc/DevSecOpsPlayground"  \
      homepage="https://github.com/umgc/DevSecOpsPlayground" \
      maintainer="umgc" \
      com.github.actions.name="SonarCloud Scan" \
      com.github.actions.description="Scan your code with SonarCloud to detect bugs, vulnerabilities and code smells in more than 25 programming languages." \
      com.github.actions.icon="check" \
      com.github.actions.color="green"

ARG SONAR_SCANNER_HOME=/opt/sonar-scanner
ARG NODEJS_HOME=/opt/nodejs

ENV PATH=${PATH}:${SONAR_SCANNER_HOME}/bin:${NODEJS_HOME}/bin

# set up local envs in order to allow for special chars (non-asci) in filenames
ENV LC_ALL="C.UTF-8"

WORKDIR /opt

# https://help.github.com/en/actions/creating-actions/dockerfile-support-for-github-actions#user
USER root

# Prepare entrypoint
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]




