import { ICrowdactionParticipant, IUser } from "../../api/types";
import React from "react";
import CrowdactionCard from "../CrowdactionCard/CrowdactionCard";
import { Card, CardActions, CardContent } from "@material-ui/core";
import { useMutation, gql } from "@apollo/client";
import { Button } from "../Button/Button";

interface ICrowdactionParticipatingProps {
    user: IUser;
    participant: ICrowdactionParticipant;
}

export default ({ user, participant }: ICrowdactionParticipatingProps) => {
    const [ toggleSubscription ] = useMutation(
        SET_CROWDACTION_SUBSCRIPTION,
        {
            variables: {
                crowdactionId: participant.crowdaction.id,
                userId: user.id,
                token: participant.unsubscribeToken,
                isSubscribed: !participant.subscribedToCrowdactionEmails
            }
        }
    );

    return <Card>
        <CardContent>
            <CrowdactionCard crowdaction={participant.crowdaction} />
        </CardContent>
        <CardActions>
            <Button onClick={() => toggleSubscription()}>{ participant.subscribedToCrowdactionEmails ? <p>Unsubscribe from crowdaction e-mails</p> : <p>Subscribe to crowdaction e-mails</p> }</Button>
        </CardActions>
    </Card>;
}

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