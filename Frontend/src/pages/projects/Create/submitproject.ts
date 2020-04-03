import { ExecutionResult, gql, useMutation } from "@apollo/client";
import { IProjectResult } from "../../../api/types";
import { IProjectForm } from "./form";

const uploadImage = async (file: any, description: string) => {
  const body = new FormData();
  body.append('Image', file);
  body.append('ImageDescription', description);

  return await fetch(`${process.env.REACT_APP_BACKEND_URL}/image`, {
    method: 'POST',
    body,
    credentials: 'include'
  }).then((response) => response.json());
};

async function useSubmitProject(form: IProjectForm): Promise<IProjectResult> {
  let bannerId;
  if (form.banner) {
    bannerId = await uploadImage(form.banner, form.projectName);
  }

  let imageId;
  if (form.image) {
    imageId = await uploadImage(form.image, form.imageDescription);
  }

  const [createProject] = useMutation(gql`
    mutation Create($project: NewProjectInputGraph) {
      project {
        createProject(project: $project) {
          succeeded
          errors {
            errorMessage
            memberNames
          }
        }
      }
    }
  `);

  const response = (await createProject({
    variables: {
      project: {
        name: form.projectName,
        bannerImageFileId: bannerId || null,
        categories: [form.category],
        target: form.target,
        proposal: form.proposal,
        description: form.description,
        start: form.startDate,
        end: form.endDate,
        goal: form.goal,
        tags: form.tags ? form.tags.split(';') : [],
        creatorComments: form.comments || null,
        descriptiveImageFileId: imageId || null,
        descriptionVideoLink: form.youtube || null
      }
    }
  })) as ExecutionResult;

  if (!response || !response.data) {
    throw new Error('No response from graphql endpoint');
  }

  return response.data.project.createProject;
}

export default useSubmitProject;