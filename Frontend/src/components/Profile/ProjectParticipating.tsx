import { IProjectParticipant, IUser } from "../../api/types";
import React from "react";
import ProjectCard from "../ProjectCard/ProjectCard";
import { Card, CardActions, CardContent, Button } from "@material-ui/core";
import { useMutation, gql } from "@apollo/client";

interface IProjectParticipatingProps {
    user: IUser;
    participant: IProjectParticipant;
}

export default ({ user, participant }: IProjectParticipatingProps) => {
    const [ toggleSubscription ] = useMutation(
        SET_PROJECT_SUBSCRIPTION,
        {
            variables: {
                projectId: participant.project.id,
                userId: user.id,
                token: participant.unsubscribeToken,
                isSubscribed: !participant.subscribedToProjectEmails
            }
        }
    );

    return <Card>
        <CardContent>
            <ProjectCard project={participant.project} />
        </CardContent>
        <CardActions>
            <Button onClick={() => toggleSubscription()}>{ participant.subscribedToProjectEmails ? <p>Unsubscribe from project e-mails</p> : <p>Subscribe to project e-mails</p> }</Button>
        </CardActions>
    </Card>;
}

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