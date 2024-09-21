#!/bin/bash
rabbitmq-server -detached

 count=0;
# Execute and check if user has been created
until timeout 5 rabbitmqctl list_users >/dev/null 2>/dev/null || (( count++ >= 10 )); do sleep 1; done;
if rabbitmqctl list_users | grep admin > /dev/null
   then
   # Create mq user and Assign admin privilages.     
   #rabbitmqctl add_user $RABBITMQ_DEFAULT_USER $RABBITMQ_DEFAULT_PASS  ; \
   rabbitmqctl set_user_tags $RABBITMQ_DEFAULT_USER administrator ; \
   rabbitmqctl set_permissions -p / $RABBITMQ_DEFAULT_USER  ".*" ".*" ".*" ; \
   echo "'$RABBITMQ_DEFAULT_USER' created sucessfully with admin previlages."

   # MQ Settings Exchange / Queue /Binding
   echo "Initiating MQ setups."
   rabbitmqadmin -u $RABBITMQ_DEFAULT_USER -p $RABBITMQ_DEFAULT_PASS declare exchange name=$KR_EXCHANGE type=topic durable=true auto_delete=false -V $RABBITMQ_DEFAULT_VHOST ; \
   rabbitmqadmin -u $RABBITMQ_DEFAULT_USER -p $RABBITMQ_DEFAULT_PASS declare exchange name=$KR_DEADLETTER_EXCHANGE type=topic durable=true auto_delete=false -V $RABBITMQ_DEFAULT_VHOST ; \
   echo "'$KR_EXCHANGE' exchange created."
   rabbitmqadmin -u $RABBITMQ_DEFAULT_USER -p $RABBITMQ_DEFAULT_PASS declare queue name=$KR_QUEUE  -V $RABBITMQ_DEFAULT_VHOST ; \
   rabbitmqadmin -u $RABBITMQ_DEFAULT_USER -p $RABBITMQ_DEFAULT_PASS declare queue name=$KR_DEADLETTER_QUEUE -V $RABBITMQ_DEFAULT_VHOST ; \
   echo "'$KR_QUEUE' queue created."
   rabbitmqadmin -u $RABBITMQ_DEFAULT_USER -p $RABBITMQ_DEFAULT_PASS declare binding source=$KR_EXCHANGE destination=$KR_QUEUE routing_key=$KR_ROUTE_KEY -V $RABBITMQ_DEFAULT_VHOST ; \
   rabbitmqadmin -u $RABBITMQ_DEFAULT_USER -p $RABBITMQ_DEFAULT_PASS declare binding source=$KR_DEADLETTER_EXCHANGE destination=$KR_DEADLETTER_QUEUE  -V $RABBITMQ_DEFAULT_VHOST ; \
   echo "Binding created."
   rabbitmqctl set_policy DLX "^$KR_QUEUE$" '{"dead-letter-exchange":"'$KR_DEADLETTER_EXCHANGE'"}' --apply-to queues --priority 0 --vhost=$RABBITMQ_DEFAULT_VHOST ; \
   echo "Binding created."

else
   echo "Process failed."
fi

tail -f /dev/null
