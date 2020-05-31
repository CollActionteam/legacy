import React from "react";
import { IUser } from "../../api/types";
import { CardContent, Card } from "@material-ui/core";
import CrowdactionParticipating from "./CrowdactionParticipating";

interface ICrowdactionsParticipatingProps {
    user: IUser;
}

export default ({ user }: ICrowdactionsParticipatingProps) => {
    return <Card>
            <CardContent>
                <h3>Crowdactions Participating</h3>
                <br />
                { 
                    user.participates.map(participant => <CrowdactionParticipating key={participant.id} user={user} participant={participant} />)
                }
                { user.participates.length === 0 ? <p>You have no crowdactions you're participating in</p> : null }
            </CardContent>
        </Card>;
};