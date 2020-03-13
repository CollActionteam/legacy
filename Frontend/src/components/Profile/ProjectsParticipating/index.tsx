import React from "react";
import { IUser } from "../../../api/types";
import { Card, CardContent, CardActions } from "@material-ui/core";

interface IProjectsParticipatingProps {
    user: IUser;
}

export default (props: IProjectsParticipatingProps) => {
    return <Card>
        <CardContent>
            <h3>Projects Participating</h3>
        </CardContent>
        <CardActions>
        </CardActions>
    </Card>
};