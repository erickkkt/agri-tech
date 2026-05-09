# base image
FROM node:20.11.1-alpine as build

ARG build_env="production"
ENV env_build_env ${build_env}
RUN echo "build_env being used: $env_build_env"

ARG API_URL=""
ENV ENV_API_URL ${API_URL}
RUN echo "API_URL being used: $ENV_API_URL"

# set working directory
WORKDIR /app

# install and cache app dependencies
COPY . /app

#RUN npm i @angular-devkit/build-angular --legacy-peer-deps
RUN npm ci

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH


# generate build
RUN ng build --configuration=$env_build_env --output-hashing=all --output-path=dist


############
### prod ###
############

# base image
FROM nginx:1.26.2-alpine

# copy artifact build from the 'build environment'
COPY --from=build /app/dist/browser /usr/share/nginx/html

# copy Angular SPA friendly config
COPY docker/nginx-conf /etc/nginx/conf.d/

# expose port 80
EXPOSE 80

# run nginx
#CMD ["nginx", "-g", "daemon off;"]
CMD sed -i "s~API_URL~$ENV_API_URL~g" /usr/share/nginx/html/assets/config.json && nginx -g "daemon off;"
