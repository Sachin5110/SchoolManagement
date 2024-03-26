# School Management - Backend 

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg?cacheSeconds=2592000 "Version")
[![Documentation](https://img.shields.io/badge/documentation-yes-brightgreen.svg)](https://schoolmanagement.com)
[![License](https://img.shields.io/badge/license-schoolmanagement-%236437FF)](https://schoolmanagement.com)

A middleware to integrate Epic SMART-on-FHIR capability into existing schoolmanagement platform.

## 🏠 [Homepage](https://schoolmanagement.com)

## This middleware is built on following components

- Backend API Server - .Net 5.0
- Frontend Server - React JS
- Docker (To build docker image & run as a container: [follow these instructions.](./README-Docker.md))

## For Local or Development environment

### Web Server must have following softwares / dependencies installed

- [x] _Web Server with Apache / Nginx_
- [x] _Git version 2.17.1 or higher installed to clone repository_
- [x] _Nodejs version 12.20.0 or higher_
- [x] _(Optional) Yarn version 1.22.5 or higher_

### Go to web directory & run following command

(Please skip this step if already cloned the repository.)

```bash
git clone <Repository_url>
```

### Install all dependencies

```sh
npm install
# or
npm i
# or
yarn
```

### Add / update following environment variables in `.env` file inside root directory. If file doesnot exist, create one. You can also use `.env.example` file or following snippet

```sh
NODE_PATH=./src
SKIP_PREFLIGHT_CHECK=true

PORT=3000
HTTPS=false
## Not required in production or running app on HTTP
# SSL_CRT_FILE=""
## Not required in production or running app on HTTP
# SSL_KEY_FILE=""

REACT_APP_API_BASE_URL="https://localhost:4000"
REACT_APP_COMPANY_NAME="School Management"
REACT_APP_COMPANY_URL="https://schoolmanagement.com"

## Datatable Configuration
REACT_APP_PER_PAGE_ITEMS=25
```

### Update env according to server details such as `PORT`, `REACT_APP_API_BASE_URL`, etc

(Please note, `SSL_CRT_FILE` & `SSL_KEY_FILE` are used in local / development environment only if you want to run node server over `HTTPS`. To run node server over `HTTPS`, set `HTTPS` to `true` in `.env` file.)

### Start frontend app in local/development environment

```sh
npm run dev
# or
yarn dev
```

### If `HTTPS` set to `true` in `.env`, application will start at `https://localhost:{PORT}`, otherwise at `http://localhost:{PORT}`

## Copyright © 2022 [School Management](https://schoolmanagement.com)
