class MainScene {
    constructor(scene) {
        this.scene = new THREE.Scene();
        this.worldObjects = {};

        this.init();    // Initialize pre set models in scene
    }

    init() {
        var geometry = new THREE.PlaneGeometry(30, 30, 30);
        var material = new THREE.MeshPhongMaterial({ map: new THREE.TextureLoader().load("textures/AmazonRoad.png"), side: THREE.DoubleSide });
        var plane = new THREE.Mesh(geometry, material);
        plane.rotation.x = Math.PI / 2.0;
        plane.position.x = 15;
        plane.position.z = 15;
        plane.position.y = -0.1;
        plane.lights = true;
        plane.receiveShadow = true;
        this.scene.add(plane);

        var SkyboxGeo = new THREE.SphereGeometry(1000, 32, 32);
        var SkyboxMat = new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("models/skybox/skybox3.jpg"), side: THREE.DoubleSide });
        var skybox = new THREE.Mesh(SkyboxGeo, SkyboxMat);
        this.scene.add(skybox);

        var Nightlight = new THREE.DirectionalLight(0xffffff, 0.2);
        Nightlight.position.x = 15;
        Nightlight.position.z = 15;
        Nightlight.position.y = 15;
        this.scene.add(Nightlight);

    }

    updateScene(command) {
        if(command.command == "update") {
            if (Object.keys(this.worldObjects).indexOf(command.parameters.guid) < 0) {
                // Load new model
                this.setupModel(command);
            }

            var object = this.worldObjects[command.parameters.guid];

            object.position.x = command.parameters.x;
            object.position.y = command.parameters.y;
            object.position.z = command.parameters.z;

            object.rotation.x = command.parameters.rotationX;
            object.rotation.y = command.parameters.rotationY;
            object.rotation.z = command.parameters.rotationZ;

            object.visible = command.parameters.visible;

            if (command.parameters.delete == true) {
                this.scene.remove(object);
            }
        }
    }

    setupModel(model) {
        // Init Three Group
        var RBgroup = new THREE.Group();
        var RKgroup = new THREE.Group();
        var TRgroup = new THREE.Group();
        var NDgroup = new THREE.Group();
        var Othergroup = new THREE.Group();

        if (model.parameters.type === "robot") {
            this.loadOBJModel("models/Drone/", "drone.obj", "models/Drone/", "drone.mtl", (obj) => {
                obj.scale.set(5, 5, 5);

                var robotlight = new THREE.SpotLight(0xf44242, 0.5, 4, 0.50, 0, 0);
                //robotlight.position = command.parameters.position;
                robotlight.position.y += -1.5;
                robotlight.target.position.set(15, -1000, 15);
                robotlight.target.updateMatrixWorld();
                robotlight.castShadow = true;
                robotlight.shadow.mapSize.width = 4096;
                robotlight.shadow.mapSize.height = 4096;
                robotlight.shadow.camera.near = 0.5;
                robotlight.shadow.camera.far = 1024;
                robotlight.shadow.camera = new THREE.OrthographicCamera(-10, 10, 10, -10, 0.5, 10);

                RBgroup.add(robotlight);

                obj.traverse(function (object) {
                    object.castShadow = true;
                    object.receiveShadow = true;
                });
                RBgroup.add(obj);
            });
        } else if (model.parameters.type === "rack") {
            this.loadOBJModel("models/rack/", "rack2.obj", "models/rack/", "rack2.mtl", (obj) => {
                obj.scale.set(2, 2, 2);
                obj.traverse(function (object) {
                    object.castShadow = true;
                    object.receiveShadow = true;
                });
                RKgroup.add(obj);
            });
        } else if (model.parameters.type === "transport") {
            this.loadOBJModel("models/transport/", "xwing2.obj", "models/transport/", "xwing2.mtl", (obj) => {
                obj.scale.set(10, 10, 10);
                obj.traverse(function (object) {
                    object.castShadow = true;
                    object.receiveShadow = true;
                });
                TRgroup.add(obj);
            });
        } else if (model.parameters.type === "node") {
            var boxgeometry = new THREE.BoxGeometry(0.1, 10.0, 0.1);
            var boxmaterial = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
            var cube = new THREE.Mesh(boxgeometry, boxmaterial);
            NDgroup.add(cube);
        } else if (model.parameters.type === "adj") {
            var adjgeometry = new THREE.BoxGeometry(0.1, 10.0, 0.1);
            var adhmaterial = new THREE.MeshBasicMaterial({ color: 0xf44242 });
            var adjcube = new THREE.Mesh(adjgeometry, adhmaterial);
            NDgroup.add(adjcube);
        } else if (model.parameters.type === "sun") {
            var sungeometry = new THREE.SphereGeometry(100, 100, 100);
            var sunmaterial = new THREE.MeshBasicMaterial({ color: 0xe5f442 });
            var sunsphere = new THREE.Mesh(sungeometry, sunmaterial);
            var sunlight = new THREE.DirectionalLight(0xffffff, 1);            
            sunlight.castShadow = true;
            sunlight.shadow.mapSize.width = 4096;
            sunlight.shadow.mapSize.height = 4096;
            sunlight.shadow.camera.near = 0.5;
            sunlight.shadow.camera.far = 1024;
            sunlight.shadow.camera = new THREE.OrthographicCamera(-100, 100, 100, -100, 0.5, 1000);

            Othergroup.add(sunsphere);
            Othergroup.add(sunlight);
        }

        // Add group to Scene
        this.scene.add(RBgroup);
        this.scene.add(RKgroup);
        this.scene.add(TRgroup);
        this.scene.add(NDgroup);
        this.scene.add(Othergroup);
        this.worldObjects[model.parameters.guid] = group;
    }

    loadOBJModel(modelPath, modelName, texturePath, textureName, onload) {
        new THREE.MTLLoader()
            .setPath(texturePath)
            .load(textureName, function (materials) {
                materials.color;
                materials.preload();

                new THREE.OBJLoader()
                    .setPath(modelPath)
                    .setMaterials(materials)
                    .load(modelName, function (object) {
                        onload(object);
                    },

                        // Progress %
                        function () { },

                        // Error
                        function (e) {
                            console.log("Error loading model");
                            console.log(e);
                        });
            });
    }
}