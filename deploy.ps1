Param(
  [string]$pr,
  [string]$aws_access_key_id,
  [string]$aws_secret_access_key
)

docker run -it --rm -e "AWS_ACCESS_KEY_ID=$aws_access_key_id" -e "AWS_SECRET_ACCESS_KEY=$aws_secret_access_key" silintl/ecs-deploy -r eu-central-1 -n CollActionTest -i 156764677614.dkr.ecr.eu-central-1.amazonaws.com/collaction:$pr -c CollActionTest