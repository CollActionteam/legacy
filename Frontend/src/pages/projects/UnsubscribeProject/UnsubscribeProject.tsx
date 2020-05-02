import { Card, CardContent } from "@material-ui/core";
import React, { useState } from "react";
import { RouteComponentProps, useLocation } from "react-router-dom";
import { Alert } from "../../../components/Alert/Alert";
import { gql, useMutation } from "@apollo/client";
import Loader from "../../../components/Loader/Loader";
import { Helmet } from "react-helmet";

type TManageProjectSubscriptionParams = {
  slug: string,
  projectId: string
}

const UnsubscribeProjectPage = ({ match } : RouteComponentProps<TManageProjectSubscriptionParams>): any => {
  const searchParams = new URLSearchParams(useLocation().search);
  const userId = searchParams.get('userId');
  const token = searchParams.get('token');
  const projectId = match.params.projectId;
  const [ isUnsubscribed, setIsUnsubscribed ] = useState(false);
  const [ error, setError ] = useState<string | null>(null);
  const [ loading, setLoading ] = useState(false);

  const [ unsubscribe ] = useMutation(
    SET_PROJECT_SUBSCRIPTION,
    {
      variables: {
        projectId: projectId,
        userId: userId,
        token: token,
        isSubscribed: false
      },
      onCompleted: () => setIsUnsubscribed(true),
      onError: (data) => {
        setError(data.message);
        console.error(data.message);
      }
    }
  );

  if (!userId || !token) {
    return <Alert type="error" text="Missing parameters, cannot manage project subscription" />
  }

  if (!loading) {
    setLoading(true);
    unsubscribe();
  }

  return <Card>
    <Helmet>
      <title>Unsubscribe Project</title>
      <meta name="description" content="Unsubscribe Project" />
    </Helmet>
    <CardContent>
      <Alert type="error" text={error} />
      { isUnsubscribed ? <h3>Successfully unsubscribed from project. To manage all your project e-mail subscriptions, log-in and visit your profile page.</h3> : 
        (loading ? <Loader /> : null) }
    </CardContent>
  </Card>;
};

const SET_PROJECT_SUBSCRIPTION = gql`
  mutation SetProjectSubscription($projectId: ID!, $userId: String!, $token: String!, $isSubscribed: Boolean!) {  
    project {
      changeProjectSubscription(projectId: $projectId, userId: $userId, token: $token, isSubscribed: $isSubscribed) {
        id
        subscribedToProjectEmails
      }
    }
  }
`;

export default UnsubscribeProjectPage;