#! /bin/bash
if [ -z "$TRAVIS_PULL_REQUEST" ] || [ "$TRAVIS_PULL_REQUEST" == "false" ]; then 
    # Don't push if it's not a pull request
    echo "Skip pushing to ECR because it's not a pull request";
    exit 0;
fi

shopt -s nocasematch
CA_REGEX='(CA-[[:digit:]]+)'

if ! [[ "$TRAVIS_PULL_REQUEST_BRANCH" =~ $CA_REGEX ]]; then
	echo "Skip pushing to ECR because branch '$TRAVIS_PULL_REQUEST_BRANCH' doesn't contain a task number in the format 'CA-n'."
    exit 0;
fi

# Use the CA-number as image tag
TAG=${BASH_REMATCH[1]}

# Build and push
echo "Building CollAction image for $ECR_REPO:$TAG"
docker build -t $ECR_REPO:$TAG CollAction

# Log in to Amazon ECR.
# This uses the environment variables AWS_ACCESS_KEY_ID and AWS_ACCESS_SECRET_KEY which are set in the .travis.yml env.secure section. 
echo "Logging on to ECR"
eval $(aws ecr get-login --no-include-email --region eu-central-1)

echo "Pushing image $ECR_REPO:$TAG"
docker push $ECR_REPO:$TAG

echo "Done!"
