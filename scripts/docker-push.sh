#! /bin/bash
echo "Building branch '$TRAVIS_BRANCH'"

# Extract CA-number from branch name
shopt -s nocasematch
CA_REGEX='(CA-[[:digit:]]+)'

if [[ "$TRAVIS_BRANCH" =~ $CA_REGEX ]]; then
    # Use the CA-number as image tag
    TAG=${BASH_REMATCH[1]}
elif [[ "$TRAVIS_BRANCH" == "master"  || "$TRAVIS_BRANCH" == "Friesland" ]]; then
    echo "Building branch '$TRAVIS_BRANCH'"

    # Use the branch name as image tag
    TAG=${TRAVIS_BRANCH}
else
    echo "Skip pushing to ECR because branch '$TRAVIS_BRANCH' doesn't contain a task-number (format CA-n), and isn't master nor Friesland branch."
    exit 0;
fi

# Build 
echo "Tagging latest CollAction image as $ECR_REPO:$TAG"
docker tag collaction:latest $ECR_REPO:$TAG

# Log in to Amazon ECR.
# This uses the environment variables AWS_ACCESS_KEY_ID and AWS_ACCESS_SECRET_KEY which are set in the .travis.yml env.secure section. 
echo "Logging on to ECR"
eval $(aws ecr get-login --no-include-email --region eu-central-1)

# Push image to ECR
echo "Pushing image $ECR_REPO:$TAG"
docker push $ECR_REPO:$TAG

echo "Done!"
