import * as React from "react";
import ProjectThumb, { IProject } from "./ProjectThumb";
import renderComponentIf from "../global/renderComponentIf";

interface IProjectPreviewProps {
    projectId: string;
}

interface IProjectPreviewState {
    project: IProject;
}

export class ProjectPreview extends React.Component<IProjectPreviewProps, IProjectPreviewState> {

    constructor(props) {
        super(props);

        this.state = {
            project: null
        };

        this.loadProject(this.props.projectId);
    }

    async loadProject(projectId: string) {
        try {
          const searchProjectRequest: Request = new Request(`/api/projects/${projectId}`);
          const fetchResult: Response = await fetch(searchProjectRequest);
          const jsonResponse = await fetchResult.json();
          this.setState({ project: jsonResponse });
        }
        catch (e) {
          console.error(`Could not load project ${projectId}`);
          console.error(e);
        }
    }

    render() {
        if (this.state.project != null) {
            return (
                <ProjectThumb {...this.state.project} />
            );
        }
        else {
            return null;
        }
    }
}

renderComponentIf(
    <ProjectPreview projectId={document.getElementById("project-preview") && document.getElementById("project-preview").dataset.projectId} />,
    document.getElementById("project-preview")
);