import { Card, CardContent } from "@material-ui/core";
import React, { useState } from "react";
import { RouteComponentProps, useLocation } from "react-router-dom";
import { Alert } from "../../../components/Alert/Alert";
import { gql, useMutation } from "@apollo/client";
import Loader from "../../../components/Loader/Loader";
import { Helmet } from "react-helmet";

type TManageCrowdactionSubscriptionParams = {
  slug: string,
  crowdactionId: string
}

const UnsubscribeCrowdactionPage = ({ match } : RouteComponentProps<TManageCrowdactionSubscriptionParams>): any => {
  const searchParams = new URLSearchParams(useLocation().search);
  const userId = searchParams.get('userId');
  const token = searchParams.get('token');
  const crowdactionId = match.params.crowdactionId;
  const [ isUnsubscribed, setIsUnsubscribed ] = useState(false);
  const [ error, setError ] = useState<string | null>(null);
  const [ loading, setLoading ] = useState(false);

  const [ unsubscribe ] = useMutation(
    SET_CROWDACTION_SUBSCRIPTION,
    {
      variables: {
        crowdactionId: crowdactionId,
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
    return <Alert type="error" text="Missing parameters, cannot manage crowdaction subscription" />
  }

  if (!loading) {
    setLoading(true);
    unsubscribe();
  }

  return <Card>
    <Helmet>
      <title>Unsubscribe Crowdaction</title>
      <meta name="description" content="Unsubscribe Crowdaction" />
    </Helmet>
    <CardContent>
      <Alert type="error" text={error} />
      { isUnsubscribed ? <h3>Successfully unsubscribed from crowdaction. To manage all your crowdaction e-mail subscriptions, log-in and visit your profile page.</h3> : 
        (loading ? <Loader /> : null) }
    </CardContent>
  </Card>;
};

const SET_CROWDACTION_SUBSCRIPTION = gql`
  mutation SetCrowdactionSubscription($crowdactionId: ID!, $userId: String!, $token: String!, $isSubscribed: Boolean!) {  
    crowdaction {
      changeCrowdactionSubscription(crowdactionId: $crowdactionId, userId: $userId, token: $token, isSubscribed: $isSubscribed) {
        id
        subscribedToCrowdactionEmails
      }
    }
  }
`;

export default UnsubscribeCrowdactionPage;