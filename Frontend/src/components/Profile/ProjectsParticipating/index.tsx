import React from "react";
import { IUser } from "../../../api/types";
import { CardContent, Card } from "@material-ui/core";
import ProjectCard from "../../ProjectCard";

interface IProjectsParticipatingProps {
    user: IUser;
}

export default ({ user }: IProjectsParticipatingProps) => {
    return <Card>
            <CardContent>
                <h3>Projects Participating</h3>
                { 
                    user.projects.map(project => <ProjectCard project={project} />)
                }
                { user.projects.length === 0 ? <p>You have no projects you're participating in</p> : null }
            </CardContent>
        </Card>;
};