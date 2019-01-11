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

    loadProject(projectId: string) {
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