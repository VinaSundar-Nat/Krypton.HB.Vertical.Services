FROM rabbitmq:3-management
LABEL maintainer="vina"

ADD ./.docker/.script/rabbit.sh /
RUN chmod a+x /rabbit.sh
ENTRYPOINT ["/rabbit.sh"]