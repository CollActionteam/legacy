import React from "react";
import { IUser } from "../../api/types";
import { CardContent, Card } from "@material-ui/core";
import CrowdactionCreated from "./CrowdactionCreated";

interface ICrowdactionsCreatedProps {
    user: IUser;
}

export default ({ user }: ICrowdactionsCreatedProps) => {
    return <Card>
            <CardContent>
                <h3>Crowdactions Created</h3>
                <br />
                { 
                    user.crowdactions.map(crowdaction => <CrowdactionCreated key={crowdaction.id} crowdaction={crowdaction} user={user} />)
                }
                { user.crowdactions.length === 0 ? <p>You have no created crowdactions</p> : null }
            </CardContent>
        </Card>;
};