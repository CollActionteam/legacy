import React from "react";
import { IUser } from "../../../api/types";
import { CardContent, Card } from "@material-ui/core";
import ProjectParticipating from "../ProjectParticipating";

interface IProjectsParticipatingProps {
    user: IUser;
}

export default ({ user }: IProjectsParticipatingProps) => {
    return <Card>
            <CardContent>
                <h3>Projects Participating</h3>
                { 
                    user.participates.map(participant => <ProjectParticipating user={user} participant={participant} />)
                }
                { user.participates.length === 0 ? <p>You have no projects you're participating in</p> : null }
            </CardContent>
        </Card>;
};